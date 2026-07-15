import { Card } from "@/shared/ui/card";
import { MaterialCard } from "./MaterialCard";
import type { MaterialItem } from "../types";

interface MaterialsListProps {
  materials: MaterialItem[];
  isManagementMode: boolean;
  onEdit: (material: MaterialItem) => void;
  onDelete: (materialId: number) => void;
}

export function MaterialsList({
  materials,
  isManagementMode,
  onEdit,
  onDelete,
}: MaterialsListProps) {
  if (materials.length === 0) return null;

  return (
    <Card className="p-6">
      <h2 className="mb-4 text-lg font-semibold">Materials</h2>
      <div className="grid gap-3 sm:grid-cols-2">
        {materials.map((material) => (
          <MaterialCard
            key={material.id}
            material={material}
            isManagementMode={isManagementMode}
            onEdit={onEdit}
            onDelete={onDelete}
          />
        ))}
      </div>
    </Card>
  );
}
