# ---------- dev: dotnet watch でホットリロード ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dev
WORKDIR /app
ENV ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_USE_POLLING_FILE_WATCHER=true \
    DOTNET_CLI_TELEMETRY_OPTOUT=1
EXPOSE 8080
# ソースはバインドマウントで /app に入る
CMD ["dotnet", "watch", "run", "--no-launch-profile"]

# ---------- build: 本番用ビルド ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY src/Api/Api.csproj Api/
RUN dotnet restore Api/Api.csproj
COPY src/Api/ Api/
RUN dotnet publish Api/Api.csproj -c Release -o /out --no-restore

# ---------- prod: 軽量ランタイム(ECRに上げるのはこれ) ----------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS prod
WORKDIR /app
COPY --from=build /out .
ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080
USER $APP_UID
ENTRYPOINT ["dotnet", "Api.dll"]