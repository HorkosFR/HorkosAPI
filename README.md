# HorkosAPI

The core API and backend services for the Horkos platform. Horkos is a citizen-led initiative dedicated to centralizing, archiving, and tracking public political data and transparency metrics.

## Getting Started

1. Copy env file:

   ```bash
   cp .env.example .env
   ```

2. Edit `.env` and fill in your secrets

3. Run:

   ```bash
   docker compose -f docker-compose.dev.yml up --build
   ```

The API will be available at `http://localhost:5198`.

## Hot Reload

The dev setup uses `dotnet watch`, so any changes to `.cs` files will automatically trigger a rebuild.

## Environment Variables

| Variable | Description |
|----------|-------------|
| `POSTGRES_DB` | PostgreSQL database name |
| `POSTGRES_USER` | PostgreSQL username |
| `POSTGRES_PASSWORD` | PostgreSQL password |
| `FRONT_URL` | Frontend URL (used in email links) |

## License

See [LICENSE](LICENSE).
