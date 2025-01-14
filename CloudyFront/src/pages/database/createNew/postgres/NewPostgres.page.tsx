import {Title} from "@mantine/core";
import React from "react";
import {NewPostgresForm} from "@/sections/database/createNew/postgres/newPostgresForm";

export function NewPostgres() {
    return (
        <div>
            <Title order={1}>New PostgreSQL server</Title>
            <NewPostgresForm />
        </div>
    );
}
