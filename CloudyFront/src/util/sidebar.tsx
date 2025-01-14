import {
    CloudyIconDatabase,
    CloudyIconWebApplication,
    CloudyIconBrowseResources,
    CloudyIconResourceGroup,
    CloudyIconHomepage
} from "@/icons/Resources";

export const sidebarItems = [
    {
        name: "Homepage",
        icon: <CloudyIconHomepage />,
        href: '/'
    },
    {
        name: "Browse resources",
        icon: <CloudyIconBrowseResources />,
    },
    {
        name: "Web applications",
        icon: <CloudyIconWebApplication />,
    },
    {
        name: "Databases",
        icon: <CloudyIconDatabase />,
    },
    {
        name: "Resource groups",
        icon: <CloudyIconResourceGroup />,
    }
]
