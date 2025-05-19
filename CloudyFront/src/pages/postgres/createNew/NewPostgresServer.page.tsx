import {Title} from "@mantine/core";
import React from "react";
import {NewPostgresServerForm} from "@/sections/database/postgres/createNew/newPostgresServerForm";

export function NewPostgresServerPage() {
    return (
        <div>
            <Title order={1}>New PostgreSQL server</Title>
            <NewPostgresServerForm />
        </div>
    );
}
