import {Box, Flex, Modal, px, Text} from "@mantine/core";
import {DashboardButton} from "@/components/DashboardButton/DashboardButton";
import {CloudyIconDatabaseMicrosoftSqlServer, CloudyIconDatabasePostgreSql} from "@/icons/Database";
import {useNavigate} from "react-router-dom";

type CreateNewDatabaseDialogProps = {
    open: boolean;
    onClose: () => void;
}

const BigIconProps = {
    style: {width: px(80), height: px(80)},
    stroke: 0.5,
}

export const CreateNewDatabaseDialog = (props: CreateNewDatabaseDialogProps) => {
    const navigate = useNavigate();
    return (
        <Modal opened={props.open} onClose={props.onClose} title="Select your database flavour" size='lg' centered>
            <Flex direction="row" wrap="wrap" gap="md" justify="space-evenly" w='100%'>
                <Box>
                    <DashboardButton
                        icon={<CloudyIconDatabasePostgreSql{...BigIconProps} />}
                        w='280px'
                        onClick={() => navigate('/postgres/new/database')}>
                        PostgreSQL
                    </DashboardButton>
                    <Text c='dimmed' size='xs' w='100%' ta='center'>(free & open source database)</Text>
                </Box>
                <Box>
                    <DashboardButton
                        icon={<CloudyIconDatabaseMicrosoftSqlServer {...BigIconProps} />}
                        w='280px'
                        onClick={() => navigate('/sqlserver/new/database')}>
                        Microsoft SQL Server
                    </DashboardButton>
                    <Text c='dimmed' size='xs' w='100%' ta='center'>(version based on your licence)</Text>
                </Box>
            </Flex>
        </Modal>
    );
}
