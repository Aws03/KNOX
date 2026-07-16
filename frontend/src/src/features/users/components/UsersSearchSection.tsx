import { useState } from "react";
import { Mail, Hash, Filter, ChevronDown, ChevronUp } from "lucide-react";
import { Button } from "@/shared/ui/button";
import { Input } from "@/shared/ui/input";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/shared/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/shared/ui/select";
import UserFilter from "./UserFilter";
import type { UserFilters, SearchType } from "../types";

type UsersSearchSectionProps = {
  searchTerm: string;
  searchType: SearchType;
  onSearchTermChange: (term: string) => void;
  onSearchTypeChange: (type: SearchType) => void;
  onSearch: () => void;
  onApplyFilters: (filters: UserFilters) => void;
  onResetFilters: () => void;
};

export const UsersSearchSection = ({
  searchTerm,
  searchType,
  onSearchTermChange,
  onSearchTypeChange,
  onSearch,
  onApplyFilters,
  onResetFilters,
}: UsersSearchSectionProps) => {
  const [isFilterOpen, setIsFilterOpen] = useState(false);

  return (
    <Card>
      <CardHeader>
        <div className="flex items-center justify-between">
          <div>
            <CardTitle>Search Users</CardTitle>
            <CardDescription>Find users by email or ID</CardDescription>
          </div>
          <Button
            variant="outline"
            size="sm"
            onClick={() => setIsFilterOpen(!isFilterOpen)}
            className="gap-2"
          >
            <Filter className="h-4 w-4" />
            Filter
            {isFilterOpen ? (
              <ChevronUp className="h-4 w-4" />
            ) : (
              <ChevronDown className="h-4 w-4" />
            )}
          </Button>
        </div>
      </CardHeader>
      <CardContent className="space-y-4">
        <div className="flex flex-col gap-3 sm:flex-row">
          <Select value={searchType} onValueChange={onSearchTypeChange}>
            <SelectTrigger className="w-[140px]">
              <SelectValue />
            </SelectTrigger>
            <SelectContent>
              <SelectItem value="email">
                <div className="flex items-center gap-2">
                  <Mail className="size-4" />
                  Email
                </div>
              </SelectItem>
              <SelectItem value="id">
                <div className="flex items-center gap-2">
                  <Hash className="size-4" />
                  User ID
                </div>
              </SelectItem>
            </SelectContent>
          </Select>
          <Input
            placeholder={
              searchType === "email" ? "Enter email..." : "Enter user ID..."
            }
            value={searchTerm}
            onChange={(e) => onSearchTermChange(e.target.value)}
            onKeyDown={(e) => e.key === "Enter" && onSearch()}
            className="flex-1"
            type={searchType === "id" ? "number" : "text"}
          />
          <Button onClick={onSearch} className="sm:w-auto">
            Search
          </Button>
        </div>

        {isFilterOpen && (
          <div className="border-t pt-4 space-y-4">
            <UserFilter onApply={onApplyFilters} onReset={onResetFilters} />
          </div>
        )}
      </CardContent>
    </Card>
  );
};
