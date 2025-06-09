import { DnsMismatchExplanation } from '@/components/ReverseProxy/DnsMismatchExplanation';
import { useDomainInput } from '@/hooks/useDomainInput';
import { Box, Stack, Title, Text, Anchor, Group, TextInput, Button, Divider } from '@mantine/core';
import { IconExternalLink } from '@tabler/icons-react';
import { useCallback, useEffect, useState } from 'react';
import {
  useGetApiResourceBaseResourcePublicAccessQuery,
  usePostApiResourceBaseResourcePublicAccessMutation,
} from '@/services/rtk/cloudyApi';

export const PublicAccessSubpage = () => {
  const { data: publicAccessConfiguration, refetch } = useGetApiResourceBaseResourcePublicAccessQuery();
  const [domainInput, setDomainInput] = useState('');
  const [loading, setLoading] = useState(false);
  const {foundMatch, dnsCheckData, debouncedDomainInput} = useDomainInput(domainInput);
  const [previousDnsCheckMatch, setPreviousDnsCheckMatch] = useState<boolean>(true);
  useEffect(() => {
    if (foundMatch !== undefined && foundMatch !== previousDnsCheckMatch) {
      setPreviousDnsCheckMatch(foundMatch);
    }
  }, [foundMatch, previousDnsCheckMatch]);
  const [updatePublicAccessConfiguration] = usePostApiResourceBaseResourcePublicAccessMutation();
  const updatePublicAccessConfigurationSubmit = useCallback(async () => {
    setLoading(true);
    try {
      await updatePublicAccessConfiguration({
        configurePublicAccessCommand: { host: domainInput },
      }).unwrap();
    } finally {
      setLoading(false);
    }
    refetch();
  }, [updatePublicAccessConfiguration, domainInput, setLoading, refetch]);
  return (
    <Stack gap='sm' p='sm'>
      <Title order={3}>Https public access</Title>
      {!(publicAccessConfiguration?.created) && <Text>You have not yet enabled specific public access on your server. Your server is still accessible over http on any domain. But to use https you will have to configure the domain below</Text>}
      { publicAccessConfiguration?.created && (
        <Box>
          <Text>Your https admin entrypoint is: <Anchor href={("https://") + publicAccessConfiguration.host} target="_blank"><Group gap={4}>{publicAccessConfiguration.host}<IconExternalLink size={12} /></Group></Anchor></Text>
          <Text>You can still access your server over http on any domain that is not taken by some other resource, but it is not recommended.</Text>
        </Box>
      )}
      <Divider />
      <Title order={3}>Update https configuration</Title>

      <Stack gap="md">
        <TextInput
          label="Domain name (you must have this domain pointing to your server IP)"
          placeholder="my_app.example.com"
          value={domainInput}
          onChange={(e) => setDomainInput(e.currentTarget.value)}
        />
        {foundMatch === false && dnsCheckData && (<DnsMismatchExplanation dnsCheckData={dnsCheckData} domainInput={debouncedDomainInput} />)}
        <Button
          loading={loading}
          disabled={!domainInput}
          onClick={updatePublicAccessConfigurationSubmit}>Connect</Button>
      </Stack>
    </Stack>
  )
}
