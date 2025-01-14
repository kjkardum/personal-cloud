import React, { forwardRef } from 'react';
import { AspectRatio, ButtonProps, Button } from '@mantine/core';

export const DashboardButton = forwardRef(
  (
    { icon, children, onClick, ...props }: ButtonProps & { icon: React.ReactNode, onClick?: ()=>any },
    ref: React.ForwardedRef<HTMLButtonElement>
  ) => {
    return (
      <AspectRatio ratio={1}>
        <Button
          variant="default"
          rightSection={icon}
          h="100%"
          w="166px"
          style={{ overflowWrap: 'anywhere' }}
          ref={ref}
          onClick={onClick}
          {...props}
        >
          {children}
        </Button>
      </AspectRatio>
    );
  }
);
