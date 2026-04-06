'use client'

import { useEffect, useState } from 'react'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Label } from '@/components/ui/label'
import { Input } from '@/components/ui/input'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { useToast } from '@/hooks/use-toast'
import { getCompanyProfile, updateCompanyProfile, type CompanyProfile } from '@/lib/api'
import { Loader2 } from 'lucide-react'

export function CompanyProfileForm() {
    const [profile, setProfile] = useState<CompanyProfile | null>(null)
    const [loading, setLoading] = useState(true)
    const [saving, setSaving] = useState(false)
    const { toast } = useToast()

    const loadProfile = async () => {
        try {
            setLoading(true)
            const data = await getCompanyProfile()
            setProfile(data)
        } catch (error: any) {
            toast({
                title: 'Error',
                description: error.message || 'Failed to load company profile',
                variant: 'destructive'
            })
        } finally {
            setLoading(false)
        }
    }

    useEffect(() => {
        loadProfile()
    }, [])

    const handleSave = async (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        if (!profile) return

        try {
            setSaving(true)
            await updateCompanyProfile({
                name: profile.name,
                contactEmail: profile.contactEmail,
                contactPhone: profile.contactPhone,
                address: profile.address,
                city: profile.city
            })
            toast({
                title: 'Success',
                description: 'Company profile updated successfully'
            })
        } catch (error: any) {
            toast({
                title: 'Error',
                description: error.message || 'Failed to update profile',
                variant: 'destructive'
            })
        } finally {
            setSaving(false)
        }
    }

    if (loading) {
        return (
            <div className="flex justify-center p-8">
                <Loader2 className="h-8 w-8 animate-spin text-primary" />
            </div>
        )
    }

    if (!profile) return null

    return (
        <Card>
            <CardHeader>
                <div className="flex items-center justify-between">
                    <div>
                        <CardTitle>Company Information</CardTitle>
                        <CardDescription>Update your company details</CardDescription>
                    </div>
                    <div className="flex flex-col items-end gap-1">
                        <Label className="text-xs text-muted-foreground uppercase tracking-wider">Current Plan</Label>
                        <Badge variant="secondary" className="px-3 py-1 text-sm font-semibold">
                            {profile.planName}
                        </Badge>
                    </div>
                </div>
            </CardHeader>
            <CardContent>
                <form onSubmit={handleSave} className="space-y-4">
                    <div className="grid gap-2">
                        <Label htmlFor="company">Company Name</Label>
                        <Input 
                            id="company" 
                            value={profile.name} 
                            onChange={e => setProfile({...profile, name: e.target.value})}
                        />
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div className="grid gap-2">
                            <Label htmlFor="email">Contact Email</Label>
                            <Input 
                                id="email" 
                                type="email" 
                                value={profile.contactEmail} 
                                onChange={e => setProfile({...profile, contactEmail: e.target.value})}
                            />
                        </div>
                        <div className="grid gap-2">
                            <Label htmlFor="phone">Phone</Label>
                            <Input 
                                id="phone" 
                                value={profile.contactPhone || ''} 
                                onChange={e => setProfile({...profile, contactPhone: e.target.value})}
                            />
                        </div>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        <div className="grid gap-2">
                            <Label htmlFor="address">Address</Label>
                            <Input 
                                id="address" 
                                value={profile.address || ''} 
                                onChange={e => setProfile({...profile, address: e.target.value})}
                            />
                        </div>
                        <div className="grid gap-2">
                            <Label htmlFor="city">City</Label>
                            <Input 
                                id="city" 
                                value={profile.city || ''} 
                                onChange={e => setProfile({...profile, city: e.target.value})}
                            />
                        </div>
                    </div>
                    <div className="flex justify-end pt-2">
                        <Button type="submit" disabled={saving}>
                            {saving ? (
                                <>
                                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                                    Saving...
                                </>
                            ) : (
                                'Save Changes'
                            )}
                        </Button>
                    </div>
                </form>
            </CardContent>
        </Card>
    )
}
