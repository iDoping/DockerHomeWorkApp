        CREATE TABLE IF NOT EXISTS users (
            id SERIAL PRIMARY KEY,
            username VARCHAR(100) NOT NULL,
            email VARCHAR(100) UNIQUE NOT NULL,
            created_at TIMESTAMP DEFAULT NOW()
        );
        INSERT INTO users (username, email) VALUES
            ('alice', 'alice@example.com'),
            ('bob', 'bob@example.com'),
            ('charlie', 'charlie@example.com')
        ON CONFLICT (email) DO NOTHING;