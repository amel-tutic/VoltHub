using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VoltHub.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddReservationOverlapConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");
            migrationBuilder.Sql("""
        ALTER TABLE reservations ADD CONSTRAINT ex_reservations_charger_overlap
            EXCLUDE USING gist (charger_id WITH =, tstzrange(start_time, end_time, '[)') WITH &&)
            WHERE (status = 'Active');
        """);
            migrationBuilder.Sql("""
        ALTER TABLE reservations ADD CONSTRAINT ex_reservations_vehicle_overlap
            EXCLUDE USING gist (vehicle_id WITH =, tstzrange(start_time, end_time, '[)') WITH &&)
            WHERE (status = 'Active');
        """);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER TABLE reservations DROP CONSTRAINT IF EXISTS ex_reservations_vehicle_overlap;");
            migrationBuilder.Sql("ALTER TABLE reservations DROP CONSTRAINT IF EXISTS ex_reservations_charger_overlap;");
        }
    }
}
