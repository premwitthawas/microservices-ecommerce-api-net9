CREATE TABLE IF NOT EXISTS public."Users"
(
    "UserID" uuid NOT NULL,
    "PersonName" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Email" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Password" character varying(50) COLLATE pg_catalog."default" NOT NULL,
    "Gender" character varying(15) COLLATE pg_catalog."default",
    CONSTRAINT "Users_pkey" PRIMARY KEY ("UserID")
);