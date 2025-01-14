import {useEffect, useState} from 'react';
import { useForm } from '@mantine/form';
import {TextInput, Checkbox, Select, Button, Divider} from '@mantine/core';

interface FormValues {
    resourceGroupId: string;
    serverName: string;
    serverPort?: number;
}

function getResourceGroups(): Promise {
    return new Promise((resolve) => {
        setTimeout(() => resolve({
            availableResourceGroups: [
                {name: "rg1", id: "a2d3-4f51ag-346h7i-8j9k12"},
                {name: "rg2-sfgs", id: "a2d3-4f51ag-346h7i-8j9112"},
                {name: "rg1-demo", id: "a2d3-4f51ag-346h7i-8j9d12"}
            ],
        }), 3000);
    });
}

export const NewPostgresForm = () => {
    const form = useForm<FormValues>({
        mode: 'uncontrolled',
        initialValues: { resourceGroupId: '', serverName: '', serverPort: undefined },
    });
    const [availableResourceGroups, setAvailableResourceGroups] = useState<{name: string, id: undefined}[]>([]);

    useEffect(() => {
        getResourceGroups().then((values) => {
            setAvailableResourceGroups(values.availableResourceGroups.map((group) => ({label: group.name, value: group.id})));
        });
    }, []);

    return (
        <form onSubmit={form.onSubmit(console.log)}>
            <Select
                label="Resource group"
                placeholder="Resource group"
                key={form.key('resourceGroupId')}
                data={availableResourceGroups}
                {...form.getInputProps('resourceGroupId')}/>
            <TextInput
                label="Server name"
                placeholder="Server name"
                key={form.key('serverName')}
                {...form.getInputProps('serverName')}
            />
            <TextInput
                type="number"
                label="Server port"
                placeholder="Server port (leave empty for default)"
                key={form.key('serverPort')}
                {...form.getInputProps('serverPort')}
            />
            <Divider my='sm'/>
            <Button type='submit'>Create database server</Button>
        </form>
    );
}
