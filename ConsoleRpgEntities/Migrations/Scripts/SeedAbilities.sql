INSERT INTO Abilities (Name, Description, AbilityType, Damage, Metric)
VALUES
('Supernatural Lift','Lift with supernatural strength','LiftAbility', 30, 1000),
('Superhuman Lift', 'Lift with superhuman strength', 'Lift Ability', 25, 750),
('Heart Stab','Stab enemy in the heart','StabAbility',30,null),
('Back Stab', 'Stab enemy in the back', 'StabAbility', 25, null);

UPDATE Abilities
SET Metric = 10
WHERE Name = 'Shove';