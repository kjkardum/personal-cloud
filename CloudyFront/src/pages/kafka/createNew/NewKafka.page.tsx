import {Title} from "@mantine/core";
import React from "react";
import { NewKafkaForm } from '@/sections/messageBroker/kafka/createNew/newKafkaForm';

export function NewKafkaPage() {
  return (
    <div>
      <Title order={1}>New Kafka cluster</Title>
      <NewKafkaForm />
    </div>
  );
}
