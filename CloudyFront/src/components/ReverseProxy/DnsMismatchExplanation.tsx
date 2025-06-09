import { DnsCheckDto } from '@/services/rtk/cloudyApi';
import { Anchor, Blockquote, Collapse, List, Stack, Text, Title } from '@mantine/core';
import { IconNetworkOff } from '@tabler/icons-react';
import { useState } from 'react';

export const DnsMismatchExplanation = ({dnsCheckData, domainInput}: {dnsCheckData: DnsCheckDto, domainInput: string}) => {
  const [expandDnsExplanation, setExpandDnsExplanation] = useState(false);
  return (
    <Blockquote color="yellow" icon={<IconNetworkOff/>} mt="xl">
      <Stack>
        <Title order={4}>You are currently accessing this interface over different IP than the one used by domain {domainInput}</Title>
        <Text>This might be fine if you expect this (e.g. you have a gateway, vpn or some other proxy in front of your server), but
          if you are not sure, <Anchor onClick={()=>setExpandDnsExplanation(true)}>read more:</Anchor></Text>
        <Collapse in={expandDnsExplanation}>
          <List>
            <List.Item>The ips behind the hostname you provided ({domainInput}) are: {dnsCheckData.ipsBehindHostname?.join(', ') || 'none'}.</List.Item>
            <List.Item>The ips behind the admin interface you are currently using are: {dnsCheckData.ipsBehindAdminHostname?.join(', ')}.</List.Item>
            <List.Item>If you are using a gateway, vpn or some other proxy in front of your server, or you are connecting to admin over local interface, this is expected.</List.Item>
            <List.Item>
              On the other hand if you are connecting to a remote virtual machine over its public IP on which you will host all your publicly accessible resources, you may want to change some configuration.
            </List.Item>
            <List.Item>On your DNS provider administrator interface (e.g. name.com, namecheap.com) go to advanced DNS configuration and set these record
              <List withPadding listStyleType="disc">
                <List.Item>Type: A, Host: @, Value: {dnsCheckData.ipsBehindHostname?.find(t => !t.includes(':')) ?? 'any IP your server is using'}
                  <List withPadding listStyleType="disc">
                    <List.Item>This will allow you to use your base domain (e.g. example.com) for public resource access</List.Item>
                  </List>
                </List.Item>
                <List.Item>Type: A, Host: *, Value: {dnsCheckData.ipsBehindHostname?.find(t => !t.includes(':')) ?? 'any IP your server is using'}
                  <List withPadding listStyleType="disc">
                    <List.Item>This will allow you to use any subdomain (e.g. myapp.example.com, mydb.example.com) for public resource access</List.Item>
                  </List>
                </List.Item>
              </List>
            </List.Item>
          </List>
        </Collapse>
      </Stack>
    </Blockquote>
  )

}
