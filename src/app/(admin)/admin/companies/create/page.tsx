import { Metadata } from "next"
import { CreateCompanyForm } from "@/components/admin/companies/CreateCompanyForm"
import { PageHeader } from "@/components/admin/page-header"

export const metadata: Metadata = {
  title: "Create Company | Platform Admin",
  description: "Create a new company and initial administrator account.",
}

export default function CreateCompanyPage() {
  return (
    <div className="flex-1 space-y-4 p-4 pt-6">
      <PageHeader
        title="Create Company"
        description="Add a new company to the platform."
      />
      <div className="flex-1">
        <CreateCompanyForm />
      </div>
    </div>
  )
}
