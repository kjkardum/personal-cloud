import React, { useCallback, useEffect, useState } from 'react';
import { IconExternalLink } from '@tabler/icons-react';
import { Anchor, Box, Button, Divider, Group, Stack, Switch, Text, TextInput, Title } from '@mantine/core';
import { DnsMismatchExplanation } from '@/components/ReverseProxy/DnsMismatchExplanation';
import { useDomainInput } from '@/hooks/useDomainInput';
import {
  useGetApiResourceBaseResourceGrafanaQuery, usePostApiResourceBaseResourceGrafanaMutation,
  usePostApiResourceReverseProxyConnectByResourceIdMutation,
} from '@/services/rtk/cloudyApi';


export const GrafanaSubpage = () => {
  const { data: grafanaConfiguration, refetch } = useGetApiResourceBaseResourceGrafanaQuery();
  const [httpsInput, setHttpsInput] = useState(false);
  const [domainInput, setDomainInput] = useState('');
  const [loading, setLoading] = useState(false);
  const {foundMatch, dnsCheckData, debouncedDomainInput} = useDomainInput(domainInput);
  const [previousDnsCheckMatch, setPreviousDnsCheckMatch] = useState<boolean>(true);
  useEffect(() => {
    if (foundMatch !== undefined && foundMatch !== previousDnsCheckMatch) {
      setPreviousDnsCheckMatch(foundMatch);
      setHttpsInput(foundMatch && !debouncedDomainInput.includes('localhost'));
    }
  }, [foundMatch, previousDnsCheckMatch]);

  const [updateGrafanaConfiguration] = usePostApiResourceBaseResourceGrafanaMutation();
  const updateGrafanaConfigurationSubmit = useCallback(async () => {
    setLoading(true);
    try {
      await updateGrafanaConfiguration({
        configureGrafanaCommand: { useHttps: httpsInput, host: domainInput },
      }).unwrap();
    } finally {
      setLoading(false);
    }
    refetch();
  }, [updateGrafanaConfiguration, httpsInput, domainInput, setLoading, refetch]);
  return (
    <Stack gap='sm' p='sm'>
      <Title order={3}>Grafana service</Title>
      {!(grafanaConfiguration?.created) && <Text>You have not yet enabled grafana on your server. Configure it below</Text>}
      { grafanaConfiguration?.created && (
        <Box>
          <Text>Your grafana instance is currently available on: <Anchor href={(grafanaConfiguration.useHttps ? "https://" : "http://") + grafanaConfiguration.host} target="_blank"><Group gap={4}>{grafanaConfiguration.host}<IconExternalLink size={12} /></Group></Anchor></Text>
          <Text>Default credentials are <code>admin:admin</code>. You can change them in the grafana interface.</Text>
        </Box>
      )}
      <Divider />
      <Title order={3}>Update grafana configuration</Title>

      <Stack gap="md">
        <TextInput
          label="Domain name (you must have this domain pointing to your server IP)"
          placeholder="my_app.example.com"
          value={domainInput}
          onChange={(e) => setDomainInput(e.currentTarget.value)}
        />
        <Switch
          label="Use HTTPS (will automatically generate and refresh certificates for you using Let's Encrypt)"
          checked={httpsInput}
          onChange={(e) => setHttpsInput(e.currentTarget.checked)}
        />
        {foundMatch === false && dnsCheckData && (<DnsMismatchExplanation dnsCheckData={dnsCheckData} domainInput={debouncedDomainInput} />)}
        <Button
          loading={loading}
          disabled={!domainInput}
          onClick={updateGrafanaConfigurationSubmit}>Connect</Button>
      </Stack>
    </Stack>
  )
}
