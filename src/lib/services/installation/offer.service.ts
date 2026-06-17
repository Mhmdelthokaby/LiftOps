import { prisma } from "@/lib/db/client";
import { NotFoundError } from "@/lib/errors";

type OfferData = {
  inspectionRequestId: string;
  installationPricePerUnit: number;
  totalInstallationPrice: number;
  notes?: string;
};

export class OfferService {
  async create(companyId: string, data: OfferData) {
    const inspection = await prisma.project.findUnique({ where: { id: data.inspectionRequestId } });
    if (!inspection) throw new NotFoundError("Inspection request", data.inspectionRequestId);

    return prisma.project.update({
      where: { id: data.inspectionRequestId },
      data: {
        installationPricePerUnit: data.installationPricePerUnit,
        totalPrice: data.totalInstallationPrice,
        notes: data.notes,
      },
    });
  }

  async approve(offerId: string): Promise<void> {
    const project = await prisma.project.findUnique({ where: { id: offerId } });
    if (!project) throw new NotFoundError("Offer", offerId);

    await prisma.project.update({
      where: { id: offerId },
      data: { status: "IN_PROGRESS" },
    });
  }

  async convertToProject(offerId: string): Promise<void> {
    const project = await prisma.project.findUnique({ where: { id: offerId } });
    if (!project) throw new NotFoundError("Offer", offerId);

    await prisma.project.update({
      where: { id: offerId },
      data: { status: "IN_PROGRESS" },
    });
  }
}
