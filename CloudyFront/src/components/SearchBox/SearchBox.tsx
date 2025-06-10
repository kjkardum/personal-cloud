import React, { useMemo, useState } from 'react';
import { Combobox, Group, rem, Text, TextInput, useCombobox } from '@mantine/core';
import {sidebarItems} from "@/util/sidebar";
import { useGetApiResourceBaseResourceQuery } from '@/services/rtk/cloudyApi';
import { TypeToIcon } from '@/util/typeToDisplay';
import { viewResourceOfType, viewVirtualResource } from '@/util/navigation';
import { useDebouncedValue } from '@mantine/hooks';
import { useNavigate } from 'react-router-dom';
import { CloudyIconDocker } from '@/icons/Resources';

export function SearchBox() {
  const navigate = useNavigate();
  const combobox = useCombobox();
  const [value, setValue] = useState('');
  const [debouncedSearch] = useDebouncedValue(value, 300);
  const {data: searchResults} = useGetApiResourceBaseResourceQuery({filterBy: debouncedSearch });
  const items = useMemo(()=>[
    ...sidebarItems,
    {
      name: "Cloudy host",
      icon: <CloudyIconDocker />,
      href: viewVirtualResource('CloudyDocker'),
    },
    ...(searchResults?.data || []).map(t => ({
        name: t.name,
        icon: TypeToIcon[t.resourceType],
        href: viewResourceOfType(t.resourceType, t.id),
      }))
  ], [searchResults]);
  const shouldFilterOptions = !items.some((item) => item.name === value);
  const filteredOptions = shouldFilterOptions
    ? items.filter((item) => item.name.toLowerCase().includes(value.toLowerCase().trim())).slice(0, 10)
    : items;

  const options = filteredOptions.map((item) => (
    <Combobox.Option value={item.name} key={item.name}>
      <Group>
        {item.icon}
        <Text>{item.name}</Text>
      </Group>
    </Combobox.Option>
  ));

  return (
    <Combobox
      onOptionSubmit={(optionValue) => {
        navigate(items.find(t => t.name === optionValue)?.href || '/');
        setValue(optionValue);
        combobox.closeDropdown();
      }}
      store={combobox}
    >
      <Combobox.Target>
        <TextInput
          placeholder="Search for resources or type anything"
          value={value}
          onChange={(event) => {
            setValue(event.currentTarget.value);
            combobox.openDropdown();
            combobox.updateSelectedOptionIndex();
          }}
          onClick={() => combobox.openDropdown()}
          onFocus={() => combobox.openDropdown()}
          onBlur={() => combobox.closeDropdown()}
          style={{ width: '50%' }}
        />
      </Combobox.Target>

      <Combobox.Dropdown>
        <Combobox.Options>
          {options.length === 0 ? <Combobox.Empty>Nothing found</Combobox.Empty> : options}
        </Combobox.Options>
      </Combobox.Dropdown>
    </Combobox>
  );
}
