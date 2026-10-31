START TRANSACTION;
CREATE TABLE IF NOT EXISTS "Languages" (
    "LanguageCode" character varying(2) NOT NULL,
    "Name" character varying(20) NOT NULL,
    CONSTRAINT "PK_Languages" PRIMARY KEY ("LanguageCode")
);

CREATE TABLE IF NOT EXISTS "Profiles" (
    "Id" uuid NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Enterprise" character varying(100) NOT NULL,
    CONSTRAINT "PK_Profiles" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "RefreshTokens" (
    "Id" uuid NOT NULL,
    "TokenHash" character varying(64) NOT NULL,
    "Owner" character varying(100) NOT NULL,
    "ExpiresAt" timestamp with time zone NOT NULL,
    "ConsumedAt" timestamp with time zone,
    CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "TimelineEntries" (
    "Id" uuid NOT NULL,
    "Organization" character varying(100) NOT NULL,
    "Category" character varying(20) NOT NULL,
    "StartDate" date NOT NULL,
    "EndDate" date,
    "Technologies" text[] NOT NULL,
    CONSTRAINT "PK_TimelineEntries" PRIMARY KEY ("Id")
);

CREATE TABLE IF NOT EXISTS "ProfileTranslations" (
    "ProfileId" uuid NOT NULL,
    "LanguageCode" character varying(2) NOT NULL,
    "Presentation" character varying(750) NOT NULL,
    "Role" character varying(50) NOT NULL,
    CONSTRAINT "PK_ProfileTranslations" PRIMARY KEY ("ProfileId", "LanguageCode"),
    CONSTRAINT "FK_ProfileTranslations_Languages_LanguageCode" FOREIGN KEY ("LanguageCode") REFERENCES "Languages" ("LanguageCode") ON DELETE CASCADE,
    CONSTRAINT "FK_ProfileTranslations_Profiles_ProfileId" FOREIGN KEY ("ProfileId") REFERENCES "Profiles" ("Id") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS "TimelineEntryTranslations" (
    "EntryId" uuid NOT NULL,
    "LanguageCode" character varying(2) NOT NULL,
    "Title" character varying(100) NOT NULL,
    "Description" character varying(1000) NOT NULL,
    CONSTRAINT "PK_TimelineEntryTranslations" PRIMARY KEY ("EntryId", "LanguageCode"),
    CONSTRAINT "FK_TimelineEntryTranslations_Languages_LanguageCode" FOREIGN KEY ("LanguageCode") REFERENCES "Languages" ("LanguageCode") ON DELETE CASCADE,
    CONSTRAINT "FK_TimelineEntryTranslations_TimelineEntries_EntryId" FOREIGN KEY ("EntryId") REFERENCES "TimelineEntries" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_ProfileTranslations_LanguageCode" ON "ProfileTranslations" ("LanguageCode");

CREATE INDEX IF NOT EXISTS "IX_TimelineEntryTranslations_LanguageCode" ON "TimelineEntryTranslations" ("LanguageCode");

COMMIT;

