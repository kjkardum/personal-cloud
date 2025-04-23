import {Anchor, Autocomplete, Button, Divider, TextInput} from "@mantine/core";
import {
    CreatePostgresDatabaseCommand,
    useGetApiResourcePostgresServerResourceQuery, useLazyGetApiResourcePostgresServerResourceByServerIdQuery,
    usePostApiResourcePostgresServerResourceByServerIdDatabaseMutation
} from "@/services/rtk/cloudyApi";
import {useForm, zodResolver} from "@mantine/form";
import {z} from "zod";
import {useEffect, useState} from "react";
import {Link, useSearchParams} from "react-router-dom";

export const NewPostgresDatabaseForm = () => {
    const [createPostgresDatabaseResource] = usePostApiResourcePostgresServerResourceByServerIdDatabaseMutation();
    const [getServerById] = useLazyGetApiResourcePostgresServerResourceByServerIdQuery();
    const [searchParams] = useSearchParams();
    useEffect(() => {
        const paramsServerId = searchParams.get('serverId')
        if (paramsServerId && paramsServerId !== serverId) {
            getServerById({serverId: paramsServerId}).unwrap()
                .then((server) => {
                    setServerId(server.id || null);
                    setServerAutocompleteInput(server.name);
                });
        }
    }, [searchParams]);
    const [serverId, setServerId] = useState<string | null>(null);
    const [serverAutocompleteInput, setServerAutocompleteInput] = useState<string>('');
    const {data: postgresServers} = useGetApiResourcePostgresServerResourceQuery({filterBy: serverAutocompleteInput});
    const form = useForm<CreatePostgresDatabaseCommand>({
        mode: 'uncontrolled',
        initialValues: { databaseName: '', adminUsername: '', adminPassword: '' },
        validate: zodResolver(
            z.object({
                databaseName: z.string().nonempty('Database name is required'),
                adminUsername: z.string().nonempty('Admin username is required'),
                adminPassword: z.string().nonempty('Admin password is required'),
            })
        ),
    });

    const submitForm = async (values: CreatePostgresDatabaseCommand) => {
        if (!serverId) {
            throw new Error('Server ID is required');
        }
        await createPostgresDatabaseResource({ createPostgresDatabaseCommand: values, serverId: serverId || '' }).unwrap();
        form.reset();
    };
    return (
        <form onSubmit={form.onSubmit(submitForm)}>
            <TextInput
                label="Database name"
                placeholder="Database name"
                key={form.key('databaseName')}
                {...form.getInputProps('databaseName')}
            />
            <Divider/>
            <Autocomplete
                label="Postgres server"
                placeholder="Search for a Postgres server"
                value={serverAutocompleteInput}
                data={
                    (postgresServers?.data??[]).map((server) => ({
                        value: server.id,
                        label: server.name,
                    }))
                }
                onChange={(value) => {
                    const foundItem = (postgresServers?.data??[]).find((server) => server.name === value);
                    if (foundItem) {
                        setServerId(foundItem.id || '');
                    } else {
                        setServerId(null);
                    }
                    setServerAutocompleteInput(value);
                }}
                />
            <Anchor component={Link} to='/postgres/new/server'>Create new</Anchor>
            <Divider/>
            <TextInput
                label="Admin username"
                placeholder="Admin username"
                key={form.key('adminUsername')}
                {...form.getInputProps('adminUsername')}
            />
            <TextInput
                label="Admin password"
                placeholder="Admin password"
                type="password"
                key={form.key('adminPassword')}
                {...form.getInputProps('adminPassword')}
            />
            <Divider my="sm" />
            <Button type="submit">Create database database</Button>
        </form>
    );
}
