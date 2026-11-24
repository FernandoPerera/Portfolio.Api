-- Sample ways of working to try the API locally. Needs 001_mock_data.sql first (languages). It can be run more than once.
START TRANSACTION;

INSERT INTO "WaysOfWorking" ("Id", "Position", "Illustration") VALUES
    ('33333333-3333-3333-3333-333333333331', 1, 'plain-language'),
    ('33333333-3333-3333-3333-333333333332', 2, 'tested'),
    ('33333333-3333-3333-3333-333333333333', 3, 'extensible'),
    ('33333333-3333-3333-3333-333333333334', 4, 'fast-response')
ON CONFLICT DO NOTHING;

INSERT INTO "WayOfWorkingTranslations" ("WayId", "LanguageCode", "Label", "Caption", "Title", "Description", "Points") VALUES
    ('33333333-3333-3333-3333-333333333331', 'es', 'Claro', 'Sin jerga', 'Te lo explico sin jerga',
        'Antes de escribir una línea hablamos de lo que necesitas con palabras normales. Si aparece un término técnico, te cuento qué significa.',
        ARRAY['Una charla corta para entender tu idea', 'Un resumen escrito de lo que acordamos', 'Avances que puedes ver y probar, no solo leer']),
    ('33333333-3333-3333-3333-333333333331', 'en', 'Clear', 'No jargon', 'I explain it without jargon',
        'Before writing a line we talk about what you need in plain words. If a technical term shows up, I tell you what it means.',
        ARRAY['A short chat to understand your idea', 'A written summary of what we agreed', 'Progress you can see and try, not only read']),
    ('33333333-3333-3333-3333-333333333332', 'es', 'Probado', 'Cada cosa se comprueba', 'Cada cosa se comprueba',
        'Cada función lleva pruebas automáticas que se ejecutan solas. Si algo se rompe, nos enteramos antes de que lo vea nadie.',
        ARRAY['Pruebas automáticas en cada cambio', 'Revisión antes de publicar', 'Errores con mensajes que se entienden']),
    ('33333333-3333-3333-3333-333333333332', 'en', 'Tested', 'Everything is checked', 'Everything is checked',
        'Every feature comes with automatic tests that run by themselves. If something breaks, we know before anyone sees it.',
        ARRAY['Automatic tests on every change', 'Review before publishing', 'Errors with messages that make sense']),
    ('33333333-3333-3333-3333-333333333333', 'es', 'Cambiable', 'Fácil de ampliar', 'Fácil de ampliar mañana',
        'El código se organiza en piezas pequeñas e independientes, para que añadir algo nuevo no obligue a rehacer lo anterior.',
        ARRAY['Piezas pequeñas y separadas', 'Decisiones apuntadas por escrito', 'Cualquiera puede continuar el trabajo']),
    ('33333333-3333-3333-3333-333333333333', 'en', 'Changeable', 'Easy to extend', 'Easy to extend tomorrow',
        'The code is organised in small, independent pieces, so adding something new does not mean redoing what is already there.',
        ARRAY['Small, separate pieces', 'Decisions written down', 'Anyone can carry on the work']),
    ('33333333-3333-3333-3333-333333333334', 'es', '< 24 h', 'Respuesta en un día', 'Respuesta en menos de un día',
        'Respondo a tu mensaje en menos de 24 horas, de lunes a viernes, y siempre te digo en qué punto estamos.',
        ARRAY['Primera respuesta en menos de 24 h', 'Un resumen de avances cada semana', 'Un único sitio donde ver el estado']),
    ('33333333-3333-3333-3333-333333333334', 'en', '< 24 h', 'Answer within a day', 'Answer in less than a day',
        'I reply to your message within 24 hours, Monday to Friday, and I always tell you where we stand.',
        ARRAY['First reply in under 24 h', 'A progress summary every week', 'A single place to see the status'])
ON CONFLICT DO NOTHING;

COMMIT;
