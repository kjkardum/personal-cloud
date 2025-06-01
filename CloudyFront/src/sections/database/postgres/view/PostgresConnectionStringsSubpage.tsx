import { PostgresDatabaseResourceDto } from '@/services/rtk/cloudyApi';
import { Box, Divider, List, ListItem, Stack, Text, Textarea, TextInput, Title } from '@mantine/core';
import React, { useState } from 'react';
import { DockerNamingHelper } from '@/util/dockerNamingHelper';


export const PostgresConnectionStringsSubpage = ({resourceBaseData}: { resourceBaseData:  PostgresDatabaseResourceDto | undefined }) => {
  const [enteredPassword, setEnteredPassword] = useState('');
  const passwordPlaceholder = enteredPassword ? enteredPassword : '***';
  const user = resourceBaseData?.adminUsername || '***';
  const host = DockerNamingHelper.getContainerName(resourceBaseData?.serverId || '***');
  const port = 5432;
  const adonetConnectionString = `Server=${host};Port=${port};Database=${resourceBaseData?.name};User Id=${user};Password=${passwordPlaceholder};`;
  const libpgConnectionString = `postgresql://${user}:${passwordPlaceholder}@${host}:${port}/${resourceBaseData?.name}`;
  const jdbcConnectionString = `jdbc:postgresql://${host}:${port}/${resourceBaseData?.name}?user=${user}&password=${passwordPlaceholder}`;
  const asyncPgConnectionString = `postgresql+asyncpg://${user}:${passwordPlaceholder}@${host}:${port}/${resourceBaseData?.name}`;
  return (
      <Stack p='sm'>
        <Box>
          <Title order={3}>Connection strings</Title>
          <List>
            <ListItem>
              Database password is not accessible by the system. You can manually replace the placeholder or use text input below to fill connection string with it.
            </ListItem>
            <ListItem>
              Hostname provided is only accessible from within the platform, by the resources which are allowed to connect to this database's internal network.
            </ListItem>
          </List>
        </Box>
        <TextInput
          maw={300}
          label={`${resourceBaseData?.adminUsername}'s password`}
          placeholder="Enter the admin password"
          type='password'
          autoComplete='current-password'
          value={enteredPassword}
          onChange={(e) => setEnteredPassword(e.currentTarget.value)}
        />
        <Divider />
        <Textarea
          variant="filled"
          label="LibPq (URL format)"
          value={libpgConnectionString}
          readOnly
          description="Use for: pgAdmin, psql, libpq, etc."
          placeholder="Input placeholder"
          />
        <Textarea
          variant="filled"
          label="JDBC"
          value={jdbcConnectionString}
          readOnly
          description="Use for: Java applications, Spring Boot, DataGrip, etc."
          placeholder="Input placeholder"
          />
        <Textarea
          variant="filled"
          label=".NET"
          value={adonetConnectionString}
          readOnly
          description="Use for: EntityFramework, Dapper, ADO.NET"
          placeholder="Input placeholder"
        />
        <Textarea
          variant="filled"
          label="AsyncPG"
          value={asyncPgConnectionString}
          readOnly
          description="Use for: Python async applications, FastAPI, etc."
          placeholder="Input placeholder"
        />
      </Stack>
  )
}
