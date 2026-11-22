START TRANSACTION;
CREATE TABLE IF NOT EXISTS "WaysOfWorking" (
    "Id" uuid NOT NULL,
    "Position" integer NOT NULL,
    "Illustration" character varying(30) NOT NULL,
    CONSTRAINT "PK_WaysOfWorking" PRIMARY KEY ("Id"),
    CONSTRAINT "UQ_WaysOfWorking_Position" UNIQUE ("Position") DEFERRABLE INITIALLY DEFERRED
);

CREATE TABLE IF NOT EXISTS "WayOfWorkingTranslations" (
    "WayId" uuid NOT NULL,
    "LanguageCode" character varying(2) NOT NULL,
    "Label" character varying(30) NOT NULL,
    "Caption" character varying(60) NOT NULL,
    "Title" character varying(100) NOT NULL,
    "Description" character varying(500) NOT NULL,
    "Points" text[] NOT NULL,
    CONSTRAINT "PK_WayOfWorkingTranslations" PRIMARY KEY ("WayId", "LanguageCode"),
    CONSTRAINT "FK_WayOfWorkingTranslations_Languages_LanguageCode" FOREIGN KEY ("LanguageCode") REFERENCES "Languages" ("LanguageCode") ON DELETE CASCADE,
    CONSTRAINT "FK_WayOfWorkingTranslations_WaysOfWorking_WayId" FOREIGN KEY ("WayId") REFERENCES "WaysOfWorking" ("Id") ON DELETE CASCADE
);

CREATE INDEX IF NOT EXISTS "IX_WayOfWorkingTranslations_LanguageCode" ON "WayOfWorkingTranslations" ("LanguageCode");

COMMIT;
