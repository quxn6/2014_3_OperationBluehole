-- Create Group Role
CREATE ROLE "OperationBluehole_Group_User"
  NOSUPERUSER INHERIT NOCREATEDB NOCREATEROLE NOREPLICATION;

-- Create DB User
CREATE USER "OperationBluehole_User"
	WITH PASSWORD 'next!@#123';
GRANT "OperationBluehole_Group_User" TO "OperationBluehole_User";

-- Create Table
CREATE TABLE accounts
(
  num serial NOT NULL,
  id character varying(16),
  password text,
  claims character varying(16)[],
  CONSTRAINT primarykey_num PRIMARY KEY (num),
  CONSTRAINT unique_id UNIQUE (id)
)
WITH (
  OIDS=FALSE
);
ALTER TABLE "accounts"
  OWNER TO postgres;
GRANT ALL ON TABLE "accounts" TO postgres;
GRANT SELECT, UPDATE, INSERT ON TABLE "accounts" TO "OperationBluehole_Group_User";

CREATE INDEX index_id
  ON "accounts"
  USING hash
  (id COLLATE pg_catalog."default");

GRANT UPDATE ON TABLE accounts_num_seq TO "OperationBluehole_Group_User";