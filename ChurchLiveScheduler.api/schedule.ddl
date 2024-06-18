CREATE TABLE `Series`
(
    `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `Name` TEXT NOT NULL,
    `Day` INTEGER NOT NULL,
    `Hours` INTEGER NOT NULL,
    `Minutes` INTEGER NOT NULL
);

CREATE TABLE `Cancellations`
(
    `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `SeriesId` INTEGER,
    `Date` TEXT NOT NULL,
    `Reason` TEXT,
    FOREIGN KEY (`SeriesId`)
        REFERENCES `Series` (`Id`)
            ON DELETE CASCADE
            ON UPDATE NO ACTION    
);

CREATE TABLE `Specials`
(
    `Id` INTEGER PRIMARY KEY AUTOINCREMENT,
    `Name` TEXT NOT NULL,
    `Datetime` TEXT NOT NULL
);

INSERT INTO `Series` VALUES (1, 'Sunday Morning Worship', 0, 10, 30);
INSERT INTO `Series` VALUES (2, 'Sunday Evening Bible Study', 0, 18, 0);
INSERT INTO `Series` VALUES (3, 'Wednesday Bible Study', 3, 18, 30);

INSERT INTO `Cancellations` VALUES (1, 2, '2024-06-16', 'Father''s Day');

INSERT INTO `Specials` VALUES (1, 'Easter Sunrise Service', '2024-03-31T07:00:00');
INSERT INTO `Specials` VALUES (1, 'Easter Service', '2024-03-31T10:30:00');
