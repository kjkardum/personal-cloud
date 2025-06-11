import { useMemo } from 'react';
import { IconCheck, IconSettingsPlus } from '@tabler/icons-react';
import {
  ActionIcon,
  Group,
  Stack,
  Table,
  TableTbody,
  TableTd,
  TableTr,
  Title,
} from '@mantine/core';
import {
  PostgresServerResourceDto,
  useGetApiResourcePostgresServerResourceByDatabaseIdDatabaseExtensionsQuery,
  usePostApiResourcePostgresServerResourceByDatabaseIdDatabaseExtensionsMutation,
} from '@/services/rtk/cloudyApi';

export const PostgresExtensionsSubpage = ({
  resourceBaseData,
}: {
  resourceBaseData: PostgresServerResourceDto | undefined;
}) => {
  const { data: extensionsData, refetch: refetchExtensions } =
    useGetApiResourcePostgresServerResourceByDatabaseIdDatabaseExtensionsQuery({
      databaseId: resourceBaseData?.id || '',
    });
  const [installExtension, {isLoading}] = usePostApiResourcePostgresServerResourceByDatabaseIdDatabaseExtensionsMutation();
  const handleInstallExtension = async (extensionName: string) => {
    if (!resourceBaseData) return;
    try {
      await installExtension({ databaseId: resourceBaseData.id, extensionName }).unwrap();
      refetchExtensions();
    } catch (error) {
      console.error('Failed to install extension:', error);
    }
  };
  const csvResponse = useMemo(
    () => [...(extensionsData?.csvResponse ?? [])] as [string, string][],
    [extensionsData]
  );
  return (
    <Stack p="sm" mih="100%">
      <Title order={3}>View or install postgres server extension</Title>
      {extensionsData && (
        <Table>
          <TableTbody>
            {csvResponse.map(([name, installedVersion], rowIndex) => (
              <TableTr key={rowIndex}>
                <TableTd>
                  <Group align="center" gap="xs">
                    {!installedVersion ? (
                      <ActionIcon
                        variant="light"
                        color="blue"
                        size="md"
                        loading={isLoading}
                        onClick={() => handleInstallExtension(name)}
                      >
                        <IconSettingsPlus />
                      </ActionIcon>
                    ) : (
                      name !== 'name' && <IconCheck color="green" />
                    )}
                    {name}
                  </Group>
                </TableTd>
                <TableTd style={{ fontWeight: 600 }}>{installedVersion}</TableTd>
              </TableTr>
            ))}
          </TableTbody>
        </Table>
      )}
    </Stack>
  );
};
