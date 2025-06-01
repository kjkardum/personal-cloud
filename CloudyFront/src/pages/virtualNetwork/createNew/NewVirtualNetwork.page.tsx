import {Title} from "@mantine/core";
import React from "react";
import { NewVirtualNetworkForm } from '@/sections/virtualNetwork/createNew/newVirtualNetworkForm';

export function NewVirtualNetworkPage() {
  return (
    <div>
      <Title order={1}>New Virtual Network</Title>
      <NewVirtualNetworkForm />
    </div>
  );
}
