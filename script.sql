USE [master]
GO
/****** Object:  Database [super]    Script Date: 2019/5/5 0:28:01 ******/
CREATE DATABASE [super]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'super', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\super.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'super_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL14.MSSQLSERVER\MSSQL\DATA\super_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
GO
ALTER DATABASE [super] SET COMPATIBILITY_LEVEL = 140
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [super].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [super] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [super] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [super] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [super] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [super] SET ARITHABORT OFF 
GO
ALTER DATABASE [super] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [super] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [super] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [super] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [super] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [super] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [super] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [super] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [super] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [super] SET  DISABLE_BROKER 
GO
ALTER DATABASE [super] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [super] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [super] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [super] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [super] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [super] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [super] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [super] SET RECOVERY FULL 
GO
ALTER DATABASE [super] SET  MULTI_USER 
GO
ALTER DATABASE [super] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [super] SET DB_CHAINING OFF 
GO
ALTER DATABASE [super] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [super] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [super] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'super', N'ON'
GO
ALTER DATABASE [super] SET QUERY_STORE = OFF
GO
USE [super]
GO
/****** Object:  Table [dbo].[epc]    Script Date: 2019/5/5 0:28:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[epc](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[epc] [nchar](24) NOT NULL,
 CONSTRAINT [PK_epc] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[mapping]    Script Date: 2019/5/5 0:28:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[mapping](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[epc] [nchar](24) NOT NULL,
	[productID] [nchar](10) NOT NULL,
 CONSTRAINT [PK_mapping] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[product]    Script Date: 2019/5/5 0:28:01 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[product](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[productID] [nchar](10) NULL,
	[name] [ntext] NULL,
	[type] [ntext] NULL,
	[unitPrice] [numeric](10, 2) NULL,
	[store] [int] NULL,
	[image] [image] NULL,
 CONSTRAINT [PK_product] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
USE [master]
GO
ALTER DATABASE [super] SET  READ_WRITE 
GO
