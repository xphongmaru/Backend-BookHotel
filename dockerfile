# =======================
# Giai đoạn Build
# =======================
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Thiết lập thư mục làm việc
WORKDIR /app

# Sao chép toàn bộ project vào container
COPY ./BookHotel /app/BookHotel

# Khôi phục các thư viện NuGet
RUN dotnet restore /app/BookHotel/BookHotel.csproj

# Build ứng dụng
RUN dotnet publish -c Release -o /app/publish /app/BookHotel/BookHotel.csproj

# =======================
# Giai đoạn Runtime
# =======================
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Thiết lập thư mục làm việc
WORKDIR /app

# Sao chép ứng dụng đã build từ giai đoạn build vào container
COPY --from=build /app/publish .

# Mở cổng 5000 để API có thể nhận request
EXPOSE 5000

# Đặt ASP.NET Core lắng nghe trên tất cả địa chỉ
ENV ASPNETCORE_URLS=http://+:5000

# Chạy ứng dụng khi container khởi động
ENTRYPOINT ["dotnet", "BookHotel.dll"]
