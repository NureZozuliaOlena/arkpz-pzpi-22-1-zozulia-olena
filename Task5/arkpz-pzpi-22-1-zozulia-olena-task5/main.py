import os
import json
import subprocess
import getpass

# Функція для виконання команд в командному рядку
def execute_command(command, use_shell=False):
    try:
        result = subprocess.run(command, capture_output=True, text=True, shell=use_shell)
        if result.returncode != 0:
            print(f"Error executing command: {command}\n{result.stderr}")
            return False
        print(result.stdout)
        return True
    except Exception as e:
        print(f"Error while running command: {e}")
        return False

# Функція для перевірки, чи встановлена програма
def check_if_installed(command):
    try:
        subprocess.run(command, stdout=subprocess.DEVNULL, stderr=subprocess.DEVNULL, shell=True)
        return True
    except FileNotFoundError:
        return False

# Функція для встановлення Chocolatey
def install_chocolatey_package_manager():
    if check_if_installed("choco"):
        print("Chocolatey is already installed")
        return

    print("Installing Chocolatey...")
    choco_install_command = (
        "%SystemRoot%\\System32\\WindowsPowerShell\\v1.0\\powershell.exe -NoProfile -InputFormat None -ExecutionPolicy Bypass "
        "-Command \"iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))\" "
        "&& SET \"PATH=%PATH%;%ALLUSERSPROFILE%\\chocolatey\\bin\""
    )

    if execute_command(choco_install_command, use_shell=True):
        print("Chocolatey installed successfully")
    else:
        print("Failed to install Chocolatey")

# Функція для встановлення .NET SDK
def install_dotnet_sdk():
    if check_if_installed("dotnet --version"):
        print(".NET SDK is already installed")
        return

    print("Installing .NET SDK...")
    dotnet_install_command = "choco install dotnet-sdk --yes"

    if execute_command(dotnet_install_command, use_shell=True):
        print(".NET SDK installed successfully")
    else:
        print("Failed to install .NET SDK")

# Функція для встановлення Microsoft SQL Server Management Studio
def install_sql_server_management_studio():
    if check_if_installed("ssms.exe"):
        print("Microsoft SQL Server Management Studio is already installed")
        return

    print("Installing Microsoft SQL Server Management Studio...")
    mssql_install_command = "choco install sql-server-management-studio --yes"

    if execute_command(mssql_install_command, use_shell=True):
        print("Microsoft SQL Server Management Studio installed successfully")
    else:
        print("Failed to install Microsoft SQL Server Management Studio")

# Функція для оновлення файлу appsettings.json
def modify_appsettings_file(file_path, db_name, host, user, password):
    try:
        print(f"Attempting to update {file_path}...")
        with open(file_path, "r") as file:
            config = json.load(file)
            print(f"Current appsettings.json content: {config}")

        connection_string = (
            f"Server={host};Database={db_name};User Id={user};Password={password};"
            "Encrypt=True;TrustServerCertificate=True"
        )

        if "ConnectionStrings" not in config:
            config["ConnectionStrings"] = {}

        config["ConnectionStrings"]["DefaultConnection"] = connection_string

        with open(file_path, "w") as file:
            json.dump(config, file, indent=4)

        print(f"Successfully updated the file: {file_path}")
    except FileNotFoundError:
        print(f"The configuration file {file_path} could not be located.")
    except json.JSONDecodeError as e:
        print(f"Failed to parse JSON in the file {file_path}: {e}")
    except PermissionError:
        print(f"Access denied while attempting to modify {file_path}. Please verify permissions.")
    except Exception as e:
        print(f"An unexpected error occurred while updating {file_path}: {e}")

# Функція для знаходження резервної копії
def locate_latest_backup(directory, extension=".bak"):
    try:
        files = [f for f in os.listdir(directory) if f.endswith(extension)]
        if not files:
            return None
        files.sort(key=lambda x: os.path.getmtime(os.path.join(directory, x)), reverse=True)
        return os.path.join(directory, files[0])
    except Exception as e:
        print(f"Error: {e}")
        return None

# Функція для відновлення бази даних з резервної копії
def restore_database_from_backup(backup_file, db_name, server, user, password):
    print("Restoring the database...")

    restore_command = (
        f"sqlcmd -S {server} -U {user} -P {password} "
        f"-Q \"ALTER DATABASE {db_name} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; "
        f"DROP DATABASE {db_name}; "
        f"RESTORE DATABASE {db_name} FROM DISK = '{backup_file}' "
        f"WITH MOVE 'SmartLunchDb' TO 'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{db_name}.mdf', "
        f"MOVE 'SmartLunchDb_log' TO 'C:\\Program Files\\Microsoft SQL Server\\MSSQL16.MSSQLSERVER\\MSSQL\\DATA\\{db_name}_log.ldf', REPLACE\""
    )

    if execute_command(restore_command, use_shell=True):
        print("Database restored successfully")
    else:
        print("Failed to restore the database")
        return False

    print("Successfully restored the database")
    return True

# Основна функція
def main():
    install_chocolatey_package_manager()
    install_dotnet_sdk()
    install_sql_server_management_studio()

    script_dir = os.path.dirname(os.path.abspath(__file__))
    migration_dir = os.path.join(script_dir, "..", "SmartLunch", "SmartLunch", "Migrations")
    migration_dir = os.path.normpath(migration_dir)

    if not os.path.exists(migration_dir):
        print(f"Directory with migration {migration_dir} not found")
        return

    backup_file = locate_latest_backup(migration_dir)
    if not backup_file:
        print(f"No backup files found in {migration_dir}")
        return

    print(f"Using backup file: {backup_file}")

    db_name = "SmartLunchDbTEST"
    server = "DESKTOP-K64RKJ3"
    user = "sa"
    password = getpass.getpass("Enter the database password: ")

    if not restore_database_from_backup(backup_file, db_name, server, user, password):
        print("Failed to configure the database")
        return

    appsettings_path = os.path.join(script_dir, "..", "SmartLunch", "SmartLunch", "appsettings.json")
    appsettings_path = os.path.normpath(appsettings_path)

    if os.path.exists(appsettings_path):
        modify_appsettings_file(appsettings_path, db_name, server, user, password)
    else:
        print(f"Configuration file {appsettings_path} not found")

    print("Setup completed successfully!")

if __name__ == "__main__":
    main()
