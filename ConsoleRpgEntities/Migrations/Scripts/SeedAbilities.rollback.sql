UPDATE Abilities
SET Metric = null
WHERE Name = 'Shove';

DELETE FROM Abilities
WHERE Name != 'Shove';