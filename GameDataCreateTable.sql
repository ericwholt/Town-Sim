CREATE TABLE  GameData(
Id INT identity(1,1) PRIMARY KEY NOT NULL,
"Day" INT NOT NULL,
Actions INT NOT NULL,
Villagers INT NOT NULL,
Houses INT NOT NULL,
Wells INT NOT NULL,
Farm INT NOT NULL,
Food INT NOT NULL,
Water INT NOT NULL,
Wood INT NOT NULL,
Stone INT DEFAULT 0 NOT NULL
);