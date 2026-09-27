IF NOT EXISTS (SELECT 1 FROM Users WHERE email = 'santyzambrana@gmail.com')
BEGIN
    INSERT INTO Users (first_name, last_name, email, phone_number, address, birthday, role)
    VALUES ('Santy', 'Zambrana', 'santyzambrana@gmail.com', NULL, NULL, NULL, 'Administrador');
    PRINT 'Usuario creado.';
END
ELSE
BEGIN
    UPDATE Users SET role = 'Administrador' WHERE email = 'santyzambrana@gmail.com';
    PRINT 'El usuario ya existia; se actualizo el rol.';
END

SELECT usuario_id, first_name, last_name, email, role FROM Users WHERE email = 'santyzambrana@gmail.com';

-- Para probar el portal de un estudiante, cambie el rol:
-- UPDATE Users SET role = 'Usuario' WHERE email = 'santyzambrana@gmail.com';
-- UPDATE Users SET role = 'Bibliotecario' WHERE email = 'santyzambrana@gmail.com';

