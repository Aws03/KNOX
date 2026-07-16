import {
  Pagination,
  PaginationContent,
  PaginationItem,
  PaginationLink,
  PaginationNext,
  PaginationPrevious,
  PaginationEllipsis,
} from "@/shared/ui/pagination";

interface SmartPaginationProps {
  pageNumber: number;
  totalPages: number;
  hasPreviousPage: boolean;
  hasNextPage: boolean;
  onPageChange: (page: number) => void;
}

const SmartPagination: React.FC<SmartPaginationProps> = ({
  pageNumber,
  totalPages,
  hasPreviousPage,
  hasNextPage,
  onPageChange,
}) => {
  return (
    <Pagination>
      <PaginationContent>
        {/* Previous */}
        <PaginationItem>
          <PaginationPrevious
            className={
              !hasPreviousPage
                ? "pointer-events-none opacity-50"
                : "cursor-pointer"
            }
            onClick={() => hasPreviousPage && onPageChange(pageNumber - 1)}
          />
        </PaginationItem>

        {/* Page Numbers */}
        {[...Array(totalPages)].map((_, i) => {
          const page = i + 1;

          // show first, last, current and pages around it
          if (
            page === 1 ||
            page === totalPages ||
            (page >= pageNumber - 1 && page <= pageNumber + 1)
          ) {
            return (
              <PaginationItem key={page}>
                <PaginationLink
                  className="cursor-pointer"
                  isActive={page === pageNumber}
                  onClick={() => onPageChange(page)}
                >
                  {page}
                </PaginationLink>
              </PaginationItem>
            );
          }

          // ellipsis
          if (page === pageNumber - 2 || page === pageNumber + 2) {
            return (
              <PaginationItem key={page}>
                <PaginationEllipsis />
              </PaginationItem>
            );
          }

          return null;
        })}

        {/* Next */}
        <PaginationItem>
          <PaginationNext
            className={
              !hasNextPage ? "pointer-events-none opacity-50" : "cursor-pointer"
            }
            onClick={() => hasNextPage && onPageChange(pageNumber + 1)}
          />
        </PaginationItem>
      </PaginationContent>
    </Pagination>
  );
};

export default SmartPagination;
