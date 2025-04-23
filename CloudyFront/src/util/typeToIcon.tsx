import {CloudyIconDatabase, CloudyIconDatabaseServer, CloudyIconResourceGroup} from "@/icons/Resources";
import {ReactElement} from "react";

export const TypeToIcon: {[key: string]: ReactElement} = {
    'PostgresServerResource': <CloudyIconDatabaseServer />,
    'PostgresDatabaseResource': <CloudyIconDatabase />,
    'ResourceGroup': <CloudyIconResourceGroup />
}
