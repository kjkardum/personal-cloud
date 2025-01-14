import React, { useState } from 'react';
import { Combobox, Group, rem, Text, TextInput, useCombobox, useMantineTheme } from '@mantine/core';
import {sidebarItems} from "@/util/sidebar";

export function SearchBox() {
  const combobox = useCombobox();
  const items = sidebarItems;
  const [value, setValue] = useState('');
  const shouldFilterOptions = !items.some((item) => item.name === value);
  const filteredOptions = shouldFilterOptions
    ? items.filter((item) => item.name.toLowerCase().includes(value.toLowerCase().trim()))
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
