import React, { useCallback, useEffect, useMemo, useState } from 'react';
import { IconNetworkOff } from '@tabler/icons-react';
import {
  Anchor,
  Blockquote,
  Button,
  Drawer,
  DrawerProps,
  Stack,
  Switch,
  TextInput,
  Text,
  Title,
  Collapse, List,
} from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import { useGetApiResourceReverseProxyPreCheckDnsQuery, usePostApiResourceReverseProxyConnectByResourceIdMutation } from '@/services/rtk/cloudyApi';
import { CanTypeUseHttp } from '@/util/proxyConfigurationUtil';


export const ReverseProxyConnectDrawer = ({
                                            resourceId,
                                            resourceType,
                                            ...props
                                          }: DrawerProps & { resourceId: string; resourceType: string }) => {
  const offerHttps = useMemo(() => CanTypeUseHttp(resourceType), [resourceType]);
  const [domainInput, setDomainInput] = useState('');
  const [debouncedDomainInput] = useDebouncedValue(domainInput, 300);
  const [httpsInput, setHttpsInput] = useState(false);
  const [loading, setLoading] = useState(false);
  const myLocationHref = useMemo(() => window.location.href, []);
  const [previousDnsCheckMatch, setPreviousDnsCheckMatch] = useState<boolean>(true);
  const { data: dnsCheckData } = useGetApiResourceReverseProxyPreCheckDnsQuery({
    url: debouncedDomainInput,
    myAdminUrl: myLocationHref,
  });
  const [connectPublicNetwork] = usePostApiResourceReverseProxyConnectByResourceIdMutation();
  const tryFindMatch = useMemo(() => {
    return dnsCheckData?.ipsBehindHostname?.some((t) =>
      dnsCheckData.ipsBehindAdminHostname?.includes(t),
    );
  }, [dnsCheckData]);
  const [expandDnsExplanation, setExpandDnsExplanation] = useState(false);
  useEffect(() => {
    if (tryFindMatch !== undefined && tryFindMatch !== previousDnsCheckMatch) {
      setPreviousDnsCheckMatch(tryFindMatch);
      setHttpsInput(offerHttps && tryFindMatch && !debouncedDomainInput.includes('localhost'));
    }
  }, [tryFindMatch]);

  const attachNetwork = useCallback(async () => {
    setLoading(true);
    try {
      console.log('Attempting to attach network with domain:', domainInput, 'and https:', httpsInput);
      await connectPublicNetwork({
        resourceId,
        connectReverseProxyCommand: { useHttps: httpsInput, urlForHost: domainInput },
      }).unwrap();
    } finally {
      setLoading(false);
    }
    props.onClose();
  }, [connectPublicNetwork, resourceId, httpsInput, domainInput, setLoading, props.onClose]);
  return (
    <Drawer
      offset={8}
      radius="sm"
      position="right"
      size="xl"
      title="Add public network domain"
      {...props}
    >
      <Stack gap="md">
        <TextInput
          label="Domain name (you must have this domain pointing to your server IP)"
          placeholder="my_app.example.com"
          value={domainInput}
          onChange={(e) => setDomainInput(e.currentTarget.value)}
        />
        {offerHttps && (
          <Switch
            label="Use HTTPS (will automatically generate and refresh certificates for you using Let's Encrypt)"
            checked={httpsInput}
            onChange={(e) => setHttpsInput(e.currentTarget.checked)}
          />
        )}
        {tryFindMatch === false && dnsCheckData && (
          <Blockquote color="yellow" icon={<IconNetworkOff/>} mt="xl">
            <Stack>
              <Title order={4}>You are currently accessing this interface over different IP than the one used by domain {debouncedDomainInput}</Title>
              <Text>This might be fine if you expect this (e.g. you have a gateway, vpn or some other proxy in front of your server), but
                if you are not sure, <Anchor onClick={()=>setExpandDnsExplanation(true)}>read more:</Anchor></Text>
                <Collapse in={expandDnsExplanation}>
                  <List>
                    <List.Item>The ips behind the hostname you provided ({debouncedDomainInput}) are: {dnsCheckData.ipsBehindHostname?.join(', ') || 'none'}.</List.Item>
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
        )}
        <Button
          loading={loading}
          disabled={!domainInput}
          onClick={attachNetwork}>Connect</Button>

      </Stack>
    </Drawer>
  );
};
