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
import { useDomainInput } from '@/hooks/useDomainInput';
import { DnsMismatchExplanation } from '@/components/ReverseProxy/DnsMismatchExplanation';


export const ReverseProxyConnectDrawer = ({
                                            resourceId,
                                            resourceType,
                                            ...props
                                          }: DrawerProps & { resourceId: string; resourceType: string }) => {
  const offerHttps = useMemo(() => CanTypeUseHttp(resourceType), [resourceType]);
  const [httpsInput, setHttpsInput] = useState(false);
  const [domainInput, setDomainInput] = useState('');
  const [loading, setLoading] = useState(false);
  const {foundMatch, dnsCheckData, debouncedDomainInput} = useDomainInput(domainInput);
  const [previousDnsCheckMatch, setPreviousDnsCheckMatch] = useState<boolean>(true);
  useEffect(() => {
    if (foundMatch !== undefined && foundMatch !== previousDnsCheckMatch) {
      setPreviousDnsCheckMatch(foundMatch);
      setHttpsInput(offerHttps && foundMatch && !debouncedDomainInput.includes('localhost'));
    }
  }, [foundMatch, previousDnsCheckMatch]);

  const [connectPublicNetwork] = usePostApiResourceReverseProxyConnectByResourceIdMutation();
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
        {foundMatch === false && dnsCheckData && (<DnsMismatchExplanation dnsCheckData={dnsCheckData} domainInput={debouncedDomainInput} />)}
        <Button
          loading={loading}
          disabled={!domainInput}
          onClick={attachNetwork}>Connect</Button>
      </Stack>
    </Drawer>
  );
};
