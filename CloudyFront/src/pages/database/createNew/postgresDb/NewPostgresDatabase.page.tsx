import {Title} from "@mantine/core";
import React from "react";
import {NewPostgresDatabaseForm} from "@/sections/database/createNew/postgres/newPostgresDatabaseForm";

export function NewPostgresDatabasePage() {
    return (
        <div>
            <Title order={1}>New PostgreSQL database</Title>
            <NewPostgresDatabaseForm />
        </div>
    );
}
