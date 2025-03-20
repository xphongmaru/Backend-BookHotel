using Microsoft.EntityFrameworkCore;
using BookHotel.Models;

namespace BookHotel.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TypeRoom> TypeRooms { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<RoomPhoto> RoomPhotos { get; set; }
        public DbSet<Amenities> Amenities { get; set; }
        public DbSet<Room_Amenities> Room_Amenities { get; set; }
        public DbSet<Guess> Guess { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Booking_Room> Booking_Rooms { get; set; }
        public DbSet<Discount> Discounts { get; set; }
        public DbSet<Booking_Discount> Booking_Discounts { get; set; }
        public DbSet<Review> Reviews { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Ignore<BaseEntity>();

            //tao 2 cot created_at va updated_at
            // Khi tạo mới, mặc định CreatedAt là CURRENT_TIMESTAMP
            // Thêm mặc định cho CreatedAt
            modelBuilder.Model.GetEntityTypes()
                .Where(e => typeof(BaseEntity).IsAssignableFrom(e.ClrType))
                .ToList()
                .ForEach(entity =>
                {
                    modelBuilder.Entity(entity.ClrType)
                        .Property("CreatedAt")
                        .HasDefaultValueSql("CURRENT_TIMESTAMP");
                });

            //typeRoom-room
            modelBuilder.Entity<Room>()
                .HasOne<TypeRoom>(r => r.TypeRoom)
                .WithMany(tr => tr.Rooms)
                .HasForeignKey(r => r.TypeRoom_id);

            //room-roomphoto
            modelBuilder.Entity<RoomPhoto>()
                .HasOne<Room>(rp => rp.Room)
                .WithMany(r => r.RoomPhotos)
                .HasForeignKey(rp => rp.Room_id);

            //room-amenities
            modelBuilder.Entity<Room_Amenities>()
                .HasKey(ra => new { ra.Room_id, ra.Amenities_id });

            modelBuilder.Entity<Room_Amenities>()
                .HasOne<Room>(ra => ra.Room)
                .WithMany(r => r.Room_Amenities)
                .HasForeignKey(ra => ra.Room_id);

            modelBuilder.Entity<Room_Amenities>()
                .HasOne<Amenities>(ra=>ra.Amenities)
                .WithMany(a=>a.Room_Amenities)
                .HasForeignKey(ra=> ra.Amenities_id);

            //guess-booking
            modelBuilder.Entity<Booking>()
                .HasOne<Guess>(b => b.Guess)
                .WithMany(g => g.Bookings)
                .HasForeignKey(br => br.Guess_id);

            //booking-room
            modelBuilder.Entity<Booking_Room>()
                .HasKey(br => new { br.Booking_id, br.Room_id });

            modelBuilder.Entity<Booking_Room>()
                .HasOne<Booking>(br => br.Booking)
                .WithMany(b => b.Booking_Rooms)
                .HasForeignKey(br => br.Booking_id);

            modelBuilder.Entity<Booking_Room>()
                .HasOne<Room>(br => br.Room)
                .WithMany(r => r.Booking_Rooms)
                .HasForeignKey(br => br.Room_id);

            //review-room
            modelBuilder.Entity<Review>()
                .HasOne<Room>(r => r.Room)
                .WithMany(re => re.Reviews)
                .HasForeignKey(r => r.Room_id);

            modelBuilder.Entity<Review>()
                .HasOne<Guess>(g => g.Guess)
                .WithMany(r => r.Reviews)
                .HasForeignKey(r => r.Guess_id);

            //booking discount
            modelBuilder.Entity<Booking_Discount>()
                .HasKey(bd => new {  bd.Booking_id, bd.Discount_id });

            modelBuilder.Entity<Booking_Discount>()
                .HasOne<Discount>(bd => bd.Discount)
                .WithMany(d => d.Booking_Discounts)
                .HasForeignKey(bd => bd.Discount_id);

            modelBuilder.Entity<Booking_Discount>()
                .HasOne<Booking>(bd => bd.Booking)
                .WithMany(d => d.Booking_Discounts)
                .HasForeignKey(bd => bd.Booking_id);

            modelBuilder.Entity<Booking_Room>()
                .Property(b => b.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Room>()
                .Property(r => r.Price)
                .HasPrecision(18, 2);

        }

        public override int SaveChanges()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
