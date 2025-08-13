INSERT INTO departments (code, description) VALUES
                                                ('010', 'BEBIDAS'),
                                                ('020', 'CONGELADOS'),
                                                ('030', 'LATICINIOS'),
                                                ('040', 'VEGETAIS')
    ON CONFLICT (code) DO NOTHING;