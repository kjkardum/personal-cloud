import { useDebouncedValue } from '@mantine/hooks';
import { useMemo } from 'react';
import { useGetApiResourceReverseProxyPreCheckDnsQuery } from '@/services/rtk/cloudyApi';

export const useDomainInput = (domainInput) => {
  const [debouncedDomainInput] = useDebouncedValue(domainInput, 300);

  const myLocationHref = useMemo(() => window.location.href, []);
  const { data: dnsCheckData } = useGetApiResourceReverseProxyPreCheckDnsQuery({
    url: debouncedDomainInput,
    myAdminUrl: myLocationHref,
  });
  const foundMatch = useMemo(() => {
    return dnsCheckData?.ipsBehindHostname?.some((t) =>
      dnsCheckData.ipsBehindAdminHostname?.includes(t),
    );
  }, [dnsCheckData]);
  return { foundMatch, dnsCheckData, debouncedDomainInput };
}
