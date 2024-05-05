export interface User {
    id?: string;
    userName?: string;
    normalizedUserName?: string;
    email?: string;
    normalizedEmail?: string;
    emailConfirmed?: boolean;
    password?: string | null;
    securityStamp?: string | null;
    concurrencyStamp?: string | null;
    phoneNumber?: string | null;
    phoneNumberConfirmed?: boolean;
    twoFactorEnabled?: boolean;
    lockoutEnd?: Date | null;
    lockoutEnabled?: boolean;
    accessFailedCount?: number;

    firstName?: string;
    lastName?: string;
    profilePicture?: string;
    streetAdderss?: string;
    city?: string;
    state?: string;
    postalCode?: string;
    joinDate?: Date;
    role?: string;
    bio?: string;
    isBlocked?: boolean;
}
