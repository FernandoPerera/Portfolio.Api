-- Sample data to try the API locally. It can be run more than once.
START TRANSACTION;

INSERT INTO "Languages" ("LanguageCode", "Name") VALUES
    ('es', 'Español'),
    ('en', 'English')
ON CONFLICT DO NOTHING;

INSERT INTO "Profiles" ("Id", "Name", "Enterprise") VALUES
    ('11111111-1111-1111-1111-111111111111', 'Fernando Perera', 'Portfolio')
ON CONFLICT DO NOTHING;

INSERT INTO "ProfileTranslations" ("ProfileId", "LanguageCode", "Presentation", "Role") VALUES
    ('11111111-1111-1111-1111-111111111111', 'es', 'Desarrollador de software apasionado por el código limpio y el diseño de APIs.', 'Ingeniero de software'),
    ('11111111-1111-1111-1111-111111111111', 'en', 'Software developer passionate about clean code and API design.', 'Software engineer')
ON CONFLICT DO NOTHING;

INSERT INTO "TimelineEntries" ("Id", "Organization", "Category", "StartDate", "EndDate", "Technologies") VALUES
    ('22222222-2222-2222-2222-222222222221', 'Acme Corp', 'work', '2023-01-01', NULL, ARRAY['C#', '.NET', 'PostgreSQL']),
    ('22222222-2222-2222-2222-222222222222', 'Globex', 'work', '2020-09-01', '2022-12-01', ARRAY['TypeScript', 'React']),
    ('22222222-2222-2222-2222-222222222223', 'Universidad de La Laguna', 'education', '2016-09-01', '2020-06-01', ARRAY['Java', 'SQL'])
ON CONFLICT DO NOTHING;

INSERT INTO "TimelineEntryTranslations" ("EntryId", "LanguageCode", "Title", "Description") VALUES
    ('22222222-2222-2222-2222-222222222221', 'es', 'Desarrollador backend', 'Diseño y desarrollo de APIs en .NET.'),
    ('22222222-2222-2222-2222-222222222221', 'en', 'Backend developer', 'Design and development of .NET APIs.'),
    ('22222222-2222-2222-2222-222222222222', 'es', 'Desarrollador frontend', 'Aplicaciones web con React.'),
    ('22222222-2222-2222-2222-222222222222', 'en', 'Frontend developer', 'Web applications with React.'),
    ('22222222-2222-2222-2222-222222222223', 'es', 'Grado en Ingeniería Informática', 'Formación universitaria en ingeniería del software.'),
    ('22222222-2222-2222-2222-222222222223', 'en', 'Computer Science degree', 'University studies in software engineering.')
ON CONFLICT DO NOTHING;

COMMIT;
