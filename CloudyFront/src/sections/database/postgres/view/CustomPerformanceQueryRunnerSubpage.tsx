import { useMemo, useRef, useState } from 'react';
import { Editor, OnMount } from '@monaco-editor/react';
import { IconInfoCircle, IconSearch } from '@tabler/icons-react';
import { editor } from 'monaco-editor';
import {
  Blockquote,
  Box,
  Button,
  Input,
  Kbd, Select,
  Stack,
  Table,
  TableTbody,
  TableTd,
  TableTr,
  Text,
  Title,
  useMantineColorScheme,
  useMantineTheme,
} from '@mantine/core';
import { useOs } from '@mantine/hooks';
import {
  PostgresDatabaseResourceDto,
  PostgresQueryResultDto,
  usePostApiResourcePostgresServerResourceDatabaseByDatabaseIdRunQueryMutation,
} from '@/services/rtk/cloudyApi';


import IStandaloneCodeEditor = editor.IStandaloneCodeEditor;
import { QueryMetricOption, QueryMetricOptions } from '@/util/postgresCustomQueries';




export const CustomPerformanceQueryRunnerSubpage =  ({resourceBaseData}: { resourceBaseData:  PostgresDatabaseResourceDto }) => {
  const [runQueryMutation] = usePostApiResourcePostgresServerResourceDatabaseByDatabaseIdRunQueryMutation();
  const [result, setResult] = useState<PostgresQueryResultDto | undefined>(undefined);
  const [selectedQuery, setSelectedQuery] = useState<QueryMetricOption | null>(null);

  const runQuery = async (query: string) => {
    try {
      const result = await runQueryMutation({
        databaseId: resourceBaseData.id,
        runPostgresDatabaseQueryCommand: {
          query
        },
      }).unwrap();
      setResult(result);
    } catch (error) {
      console.error('Error running query:', error);
      setResult(undefined);
    }
  }
  const handleSelectQuery = async (_, selectedOption: QueryMetricOption | null) => {
    const option = QueryMetricOptions.find(option => option.label === selectedOption?.label) || null
    setSelectedQuery(()=>option);
    if (option) {
      await runQuery(option.value);
    } else {
      setResult(undefined);
    }
  }

  return (
    <Stack p="sm" mih="100%">
      <Title order={3}>Performance inspector</Title>
      <Select
        label="Choose metric to inspect"
        placeholder="metric..."
        data={QueryMetricOptions}
        allowDeselect={false}
        value={selectedQuery?.value}
        onChange={handleSelectQuery}
      />
      {result && (
        <>
          <Blockquote color="lightblue" icon={<IconInfoCircle/>} mt="xl">
            {selectedQuery?.description}
          </Blockquote>
        <Table>
          <TableTbody>
            {(result.csvResponse ?? []).map((row, rowIndex) => (
              <TableTr key={rowIndex}>
                {row.map((cell, cellIndex) => (
                  <TableTd key={cellIndex} style={{fontWeight: rowIndex ? 'unset' : 600}}>{cell}</TableTd>
                ))}
              </TableTr>
            ))}
          </TableTbody>
        </Table>
        </>
      )}
    </Stack>
  );
}
