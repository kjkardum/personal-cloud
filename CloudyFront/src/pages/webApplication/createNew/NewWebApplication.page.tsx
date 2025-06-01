import {Title} from "@mantine/core";
import React from "react";
import { NewWebApplicationForm } from '@/sections/webApplication/createNew/newWebApplicationForm';

export function NewWebApplicationPage() {
  return (
    <div>
      <Title order={1}>New Web application</Title>
      <NewWebApplicationForm />
    </div>
  );
}
