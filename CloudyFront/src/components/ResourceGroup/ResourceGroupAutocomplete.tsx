import {useMemo, useState} from 'react';
import { Autocomplete, AutocompleteProps } from '@mantine/core';
import { useDebouncedValue } from '@mantine/hooks';
import {
  useGetApiResourceResourceGroupQuery,
  usePostApiResourceResourceGroupMutation,
} from '@/services/rtk/cloudyApi';

type ResourceGroupAutocompleteProps = {
  onResourceGroupSelect: (resourceGroupId: string) => void;
} & AutocompleteProps;

export const ResourceGroupAutocomplete = ({
  onResourceGroupSelect,
  ...restProps
}: ResourceGroupAutocompleteProps) => {
  const [searchValue, setSearchValue] = useState('');
  const [debouncedSearch] = useDebouncedValue(searchValue, 300);

  const { data, refetch: refetchResourceGroups } = useGetApiResourceResourceGroupQuery({
    filterBy: debouncedSearch,
  });
  const [createResourceGroup] = usePostApiResourceResourceGroupMutation();

  const checkIfCreate = async (value: string) => {
    if (value === searchValue) {
      const group = await createResourceGroup({
        createResourceGroupCommand: {
          name: searchValue,
        },
      }).unwrap();
      onResourceGroupSelect(group.id);
      setSearchValue(group.name);
      refetchResourceGroups();
    }
  };

  // Transform data for Autocomplete
  const availableResourceGroups = useMemo(() =>
    data?.data?.map((group) => ({
      value: group.id,
      label: group.name,
    })) ?? [], [data]);

  return (
    <Autocomplete
      label="Resource group"
      placeholder="Search for a resource group"
      value={searchValue}
      data={
        availableResourceGroups.length
          ? availableResourceGroups
          : [{ label: `Create new "${searchValue}" resource group`, value: searchValue }]
      }
      onOptionSubmit={checkIfCreate}
      onChange={(value) => {
        setSearchValue(value);
        const rg = availableResourceGroups.find((group) => group.label === value);
        if (rg) {
          onResourceGroupSelect(rg.value);
        }
      }}
      {...restProps}
    />
  );
};
