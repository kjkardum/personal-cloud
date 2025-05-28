namespace CloudyPoC;
using Docker.DotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;

public class DockerSubnetAllocator(DockerClient dockerClient, string baseCidr = "172.17.0.1/16")
{
    private readonly IPNetwork _baseNetwork = IPNetwork.Parse(baseCidr);

    public async Task<string> GetNextAvailableSubnet(string size = "/24")
    {
        int newPrefixLength = int.Parse(size.TrimStart('/'));
        var usedSubnets = await GetDockerUsedSubnets();
        var hostSubnets = GetHostSubnets();
        var allUsed = usedSubnets.Concat(hostSubnets).ToList();

        foreach (var candidate in _baseNetwork.Subnet(newPrefixLength))
        {
            if (!allUsed.Any(used => used.Overlap(candidate)))
            {
                return candidate.ToString();
            }
        }

        throw new Exception($"No available subnet found in base CIDR {_baseNetwork} with size {size}");
    }

    private async Task<List<IPNetwork>> GetDockerUsedSubnets()
    {
        var networks = await dockerClient.Networks.ListNetworksAsync();
        var used = new List<IPNetwork>();

        foreach (var net in networks)
        {
            var ipamConfigs = net.IPAM?.Config;
            if (ipamConfigs == null) continue;

            foreach (var cfg in ipamConfigs)
            {
                if (!string.IsNullOrWhiteSpace(cfg.Subnet))
                {
                    if (IPNetwork.TryParse(cfg.Subnet, out var parsed))
                        used.Add(parsed);
                }
            }
        }

        return used;
    }

    private List<IPNetwork> GetHostSubnets()
    {
        var subnets = new List<IPNetwork>();

        foreach (var iface in NetworkInterface.GetAllNetworkInterfaces())
        {
            var props = iface.GetIPProperties();
            foreach (var addr in props.UnicastAddresses)
            {
                if (addr.Address.AddressFamily == AddressFamily.InterNetwork)
                {
                    var subnet = IPNetwork.FromAddressAndMask(addr.Address, addr.IPv4Mask);
                    subnets.Add(subnet);
                }
            }
        }

        return subnets;
    }
}

public class IPNetwork
{
    public IPAddress Network { get; }
    public int Cidr { get; }

    public static bool TryParse(string cidr, out IPNetwork network)
    {
        var parts = cidr.Split('/');
        if (parts.Length != 2 ||
            !IPAddress.TryParse(parts[0], out var ip) ||
            !int.TryParse(parts[1], out var cidrVal))
        {
            network = null;
            return false;
        }

        network = new IPNetwork(ip, cidrVal);
        return true;
    }

    public static IPNetwork Parse(string cidr)
    {
        if (!TryParse(cidr, out var network))
            throw new FormatException("Invalid CIDR format: " + cidr);
        return network;
    }

    public IPNetwork(IPAddress baseAddress, int cidr)
    {
        Network = baseAddress;
        Cidr = cidr;
    }

    public override string ToString() => $"{Network}/{Cidr}";

    public static IPNetwork FromAddressAndMask(IPAddress address, IPAddress mask)
    {
        var maskBytes = mask.GetAddressBytes();
        var ipBytes = address.GetAddressBytes();
        var netBytes = new byte[4];

        for (int i = 0; i < 4; i++)
            netBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);

        var netIP = new IPAddress(netBytes);
        int cidr = maskBytes.SelectMany(b => Convert.ToString(b, 2).PadLeft(8, '0')).Count(c => c == '1');

        return new IPNetwork(netIP, cidr);
    }

    public IEnumerable<IPNetwork> Subnet(int newPrefixLength)
    {
        if (newPrefixLength <= Cidr)
            throw new ArgumentException("New prefix length must be greater than existing");

        int numSubnets = 1 << (newPrefixLength - Cidr);
        uint baseInt = ToUInt32(Network);

        for (int i = 0; i < numSubnets; i++)
        {
            uint subnetBase = baseInt + (uint)(i << (32 - newPrefixLength));
            yield return new IPNetwork(FromUInt32(subnetBase), newPrefixLength);
        }
    }

    public bool Overlap(IPNetwork other)
    {
        return Contains(other.Network) || other.Contains(this.Network);
    }

    public bool Contains(IPAddress ip)
    {
        uint baseInt = ToUInt32(Network);
        uint ipInt = ToUInt32(ip);
        uint mask = uint.MaxValue << (32 - Cidr);
        return (baseInt & mask) == (ipInt & mask);
    }

    private static uint ToUInt32(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        return ((uint)bytes[0] << 24) | ((uint)bytes[1] << 16) | ((uint)bytes[2] << 8) | bytes[3];
    }

    private static IPAddress FromUInt32(uint ip)
    {
        return new IPAddress(new[] {
            (byte)((ip >> 24) & 0xFF),
            (byte)((ip >> 16) & 0xFF),
            (byte)((ip >> 8) & 0xFF),
            (byte)(ip & 0xFF),
        });
    }
}
