import { useMemo, useRef, useState } from 'react';
import { Editor, OnMount } from '@monaco-editor/react';
import { IconSearch } from '@tabler/icons-react';
import { editor } from 'monaco-editor';
import {
  Box,
  Button,
  Input,
  Kbd,
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




export const QueryRunnerSubpage =  ({resourceBaseData}: { resourceBaseData:  PostgresDatabaseResourceDto | undefined }) => {
  const editorRef = useRef<IStandaloneCodeEditor | null>(null);
  const theme = useMantineTheme();
  const os = useOs();
  const { colorScheme } = useMantineColorScheme();
  const [runQueryMutation, {isLoading}] = usePostApiResourcePostgresServerResourceDatabaseByDatabaseIdRunQueryMutation();
  const [result, setResult] = useState<PostgresQueryResultDto | undefined>(undefined);
  const [searchQuery, setSearchQuery] = useState<string>('');
  const filteredDataset = useMemo(() => {
    if (!result?.csvResponse) { return []; }
    if (!searchQuery) {
      return result.csvResponse;
    }
    return result.csvResponse.filter(((row, index) => !index || row.some(cell => cell.toLowerCase().includes(searchQuery.toLowerCase()))));
  }, [result, searchQuery]);

  const handleEditorDidMount: OnMount = (editor, monaco) => {
    editorRef.current = editor;
    editor.addAction({
      id: 'run-query',
      label: 'Run Query',
      keybindings: [monaco.KeyMod.CtrlCmd | monaco.KeyCode.Enter],
      run: runQuery,
    })
  }
  const getCurrentValue = () => editorRef.current?.getValue();
  const runQuery = async () => {
    if (!resourceBaseData) { return; }
    const query = getCurrentValue();
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

  return (
    <Stack p="sm" mih="100%">
      <Title order={3}>Browser based SQL Query runner</Title>
      <Box style={{ border: `1px solid ${theme.colors[theme.primaryColor][1]}` }}>
        <Editor
          theme={colorScheme === 'dark' ? 'vs-dark' : 'light'}
          height="30vh"
          defaultLanguage="pgsql"
          defaultValue={`-- Write your SQL query here --\n-- example for viewing all tables:\n-- SELECT * FROM pg_tables WHERE schemaname = '${resourceBaseData?.name}'\n`}
          onMount={handleEditorDidMount}
        />
      </Box>
      <Box>
        <Button onClick={runQuery} loading={isLoading} mr="md">
          <Text mr="xs">Run query</Text>
          <Box>
            <Kbd>{os === 'macos' ? '⌘' : 'Ctrl'}</Kbd> + <Kbd>Enter</Kbd>
          </Box>
        </Button>
        <Input
          mr="xs"
          placeholder="Search dataset"
          leftSection={<IconSearch size={16} />}
          display="inline-block"
          value={searchQuery}
          onChange={(e) => setSearchQuery(e.currentTarget.value)}
        />
        <Text display='inline-block' c='dimmed' size='xs' >
          Showing {filteredDataset.length} rows of {result?.csvResponse?.length || 0} total rows.
        </Text>
      </Box>
      {result && (
        <Table>
          <TableTbody>
            {filteredDataset.map((row, rowIndex) => (
              <TableTr key={rowIndex}>
                {row.map((cell, cellIndex) => (
                  <TableTd key={cellIndex} style={{fontWeight: rowIndex ? 'unset' : 600}}>{cell}</TableTd>
                ))}
              </TableTr>
            ))}
          </TableTbody>
        </Table>
      )}
    </Stack>
  );
}
