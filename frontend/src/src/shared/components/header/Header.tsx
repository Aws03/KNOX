import { useTranslation } from "react-i18next";
import { MenuIcon, LogOut, User } from "lucide-react";

import { Button } from "@/shared/ui/button";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/shared/ui/dropdown-menu";
import {
  NavigationMenu,
  NavigationMenuItem,
  NavigationMenuLink,
  NavigationMenuList,
} from "@/shared/ui/navigation-menu";
import { Avatar, AvatarFallback, AvatarImage } from "@/shared/ui/avatar";
import {
  Tooltip,
  TooltipContent,
  TooltipTrigger,
  TooltipProvider,
} from "@/shared/ui/tooltip";

import { cn } from "@/lib/utils";

import Logo from "@/assets/logo";
import { ModeToggle } from "../../ui/modetoggle";
import LanguageToggle from "./LanguageToggle";
import { useAuth } from "@/app/providers/useAuth";

export type NavigationSection = {
  title: string;
  href: string;
};

type HeaderProps = {
  navigationData: NavigationSection[];
  className?: string;
};

const Header = ({ navigationData, className }: HeaderProps) => {
  const { t } = useTranslation();
  const { user, isAuthenticated, logout } = useAuth();

  const getInitials = (name: string) => {
    return name
      .split(" ")
      .map((n) => n[0])
      .join("")
      .toUpperCase()
      .slice(0, 2);
  };

  const displayName = user?.fullName || user?.name || "User";

  return (
    <TooltipProvider>
      <header
        className={cn(
          "bg-background/80 backdrop-blur-md sticky top-0 z-50 h-16 border-b border-border/40 shadow-sm transition-all duration-300",
          className
        )}
      >
        <div className="mx-auto flex h-full max-w-7xl items-center justify-between px-4 sm:px-6 lg:px-8">
          {/* Logo */}
          <a
            href="/"
            className="flex items-center space-x-2 transition-transform duration-200 hover:scale-105"
            aria-label="Home"
            title="Home"
          >
            <Logo />
            <span className="sr-only">Home</span>
          </a>

          {/* Navigation - Centered */}
          <NavigationMenu className="absolute left-1/2 -translate-x-1/2 max-md:hidden">
            <NavigationMenuList className="flex items-center gap-1">
              {navigationData.map((navItem) => (
                <NavigationMenuItem key={navItem.title}>
                  <NavigationMenuLink
                    href={navItem.href}
                    className="text-muted-foreground hover:text-foreground relative px-4 py-2 text-sm font-medium transition-all duration-200 hover:bg-accent/50 rounded-lg [&>span]:hover:w-3/4"
                  >
                    {navItem.title}
                    <span className="absolute bottom-0 left-1/2 -translate-x-1/2 w-0 h-0.5 bg-primary rounded-full transition-all duration-300" />
                  </NavigationMenuLink>
                </NavigationMenuItem>
              ))}
            </NavigationMenuList>
          </NavigationMenu>

          {/* Buttons - Desktop */}
          <div className="flex items-center gap-3 max-md:hidden">
            <LanguageToggle />
            <ModeToggle />
            {isAuthenticated ? (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button
                    variant="ghost"
                    className="relative h-10 w-10 rounded-full ring-2 ring-transparent hover:ring-primary/20 transition-all duration-200"
                  >
                    <Avatar className="h-10 w-10 border-2 border-primary/10">
                      {user?.profilePictureUrl && (
                        <AvatarImage
                          src={user.profilePictureUrl}
                          alt={displayName}
                          className="object-cover"
                        />
                      )}
                      <AvatarFallback className="bg-linear-to-br from-primary to-primary/70 text-primary-foreground font-semibold">
                        {user ? getInitials(displayName) : "U"}
                      </AvatarFallback>
                    </Avatar>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent
                  className="w-64 p-2 rounded-xl shadow-lg border-border/50"
                  align="end"
                  sideOffset={8}
                >
                  <div className="flex items-center gap-3 p-3 rounded-lg bg-accent/30">
                    <Avatar className="h-12 w-12 border-2 border-primary/20">
                      {user?.profilePictureUrl && (
                        <AvatarImage
                          src={user.profilePictureUrl}
                          alt={displayName}
                          className="object-cover"
                        />
                      )}
                      <AvatarFallback className="bg-linear-to-br from-primary to-primary/70 text-primary-foreground font-semibold">
                        {user ? getInitials(displayName) : "U"}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex flex-col space-y-0.5">
                      <p className="text-sm font-semibold leading-none">
                        {displayName}
                      </p>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <p className="text-xs leading-none text-muted-foreground truncate max-w-[150px] cursor-default">
                            {user?.email}
                          </p>
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>{user?.email}</p>
                        </TooltipContent>
                      </Tooltip>
                      {user?.role && (
                        <span className="inline-flex items-center mt-1 px-2 py-0.5 rounded-full text-xs font-medium bg-primary/10 text-primary w-fit">
                          {user.role === "User" ? "Student" : user.role}
                        </span>
                      )}
                    </div>
                  </div>
                  {user?.universityName && (
                    <p className="px-3 py-2 text-xs text-muted-foreground">
                      {user.universityName}
                    </p>
                  )}
                  <DropdownMenuSeparator className="my-2" />
                  <DropdownMenuItem
                    asChild
                    className="rounded-lg cursor-pointer"
                  >
                    <a
                      href="/profile"
                      className="flex items-center gap-2 px-3 py-2"
                    >
                      <User className="h-4 w-4" />
                      {t("header.userMenu.profile")}
                    </a>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator className="my-2" />
                  <DropdownMenuItem
                    onClick={logout}
                    className="rounded-lg cursor-pointer text-destructive hover:text-destructive focus:text-destructive"
                  >
                    <LogOut className="mr-2 h-4 w-4" />
                    {t("header.userMenu.logout")}
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            ) : (
              <Button
                className="rounded-full px-6 font-medium shadow-sm hover:shadow-md transition-all duration-200"
                asChild
              >
                <a href="/auth/login">{t("auth.login.submitButton")}</a>
              </Button>
            )}
          </div>

          {/* Navigation for small screens */}
          <div className="flex items-center gap-2 md:hidden">
            {isAuthenticated ? (
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Button
                    variant="ghost"
                    className="relative h-9 w-9 rounded-full ring-2 ring-transparent hover:ring-primary/20 transition-all duration-200"
                  >
                    <Avatar className="h-9 w-9 border-2 border-primary/10">
                      {user?.profilePictureUrl && (
                        <AvatarImage
                          src={user.profilePictureUrl}
                          alt={displayName}
                          className="object-cover"
                        />
                      )}
                      <AvatarFallback className="bg-linear-to-br from-primary to-primary/70 text-primary-foreground font-semibold text-sm">
                        {user ? getInitials(displayName) : "U"}
                      </AvatarFallback>
                    </Avatar>
                  </Button>
                </DropdownMenuTrigger>
                <DropdownMenuContent
                  className="w-64 p-2 rounded-xl shadow-lg border-border/50"
                  align="end"
                  sideOffset={8}
                >
                  <div className="flex items-center gap-3 p-3 rounded-lg bg-accent/30">
                    <Avatar className="h-11 w-11 border-2 border-primary/20">
                      {user?.profilePictureUrl && (
                        <AvatarImage
                          src={user.profilePictureUrl}
                          alt={displayName}
                          className="object-cover"
                        />
                      )}
                      <AvatarFallback className="bg-linear-to-br from-primary to-primary/70 text-primary-foreground font-semibold">
                        {user ? getInitials(displayName) : "U"}
                      </AvatarFallback>
                    </Avatar>
                    <div className="flex flex-col space-y-0.5">
                      <p className="text-sm font-semibold leading-none">
                        {displayName}
                      </p>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <p className="text-xs leading-none text-muted-foreground truncate max-w-[150px] cursor-default">
                            {user?.email}
                          </p>
                        </TooltipTrigger>
                        <TooltipContent>
                          <p>{user?.email}</p>
                        </TooltipContent>
                      </Tooltip>
                      {user?.role && (
                        <span className="inline-flex items-center mt-1 px-2 py-0.5 rounded-full text-xs font-medium bg-primary/10 text-primary w-fit">
                          {user.role === "User" ? "Student" : user.role}
                        </span>
                      )}
                    </div>
                  </div>
                  {user?.universityName && (
                    <p className="px-3 py-2 text-xs text-muted-foreground">
                      {user.universityName}
                    </p>
                  )}
                  <DropdownMenuSeparator className="my-2" />
                  <DropdownMenuItem
                    asChild
                    className="rounded-lg cursor-pointer"
                  >
                    <a
                      href="/profile"
                      className="flex items-center gap-2 px-3 py-2"
                    >
                      <User className="h-4 w-4" />
                      {t("header.userMenu.profile")}
                    </a>
                  </DropdownMenuItem>
                  <DropdownMenuSeparator className="my-2" />
                  <DropdownMenuItem
                    onClick={logout}
                    className="rounded-lg cursor-pointer text-destructive hover:text-destructive focus:text-destructive"
                  >
                    <LogOut className="mr-2 h-4 w-4" />
                    {t("header.userMenu.logout")}
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            ) : (
              <Button
                className="rounded-full px-5 h-9 text-sm font-medium shadow-sm"
                asChild
              >
                <a href="/auth/login">{t("auth.login.submitButton")}</a>
              </Button>
            )}
            <ModeToggle />
            <LanguageToggle />

            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  variant="outline"
                  size="icon"
                  className="rounded-lg border-border/50 hover:bg-accent/50 transition-all duration-200"
                >
                  <MenuIcon className="h-5 w-5" />
                  <span className="sr-only">Menu</span>
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent
                className="w-56 p-2 rounded-xl shadow-lg border-border/50"
                align="end"
                sideOffset={8}
              >
                {navigationData.map((item, index) => (
                  <DropdownMenuItem
                    key={index}
                    asChild
                    className="rounded-lg cursor-pointer"
                  >
                    <a href={item.href} className="px-3 py-2 font-medium">
                      {item.title}
                    </a>
                  </DropdownMenuItem>
                ))}
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </div>
      </header>
    </TooltipProvider>
  );
};

export default Header;
