"use client"

import type { FormEvent } from "react"
import { useState } from "react"
import Link from "next/link"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Mail, Phone, MapPin } from "lucide-react"

export default function ContactPage() {
  const [isSubmitting, setIsSubmitting] = useState(false)

  const handleSubmit = async (e: FormEvent<HTMLFormElement>) => {
    e.preventDefault()
    setIsSubmitting(true)
    setTimeout(() => {
      setIsSubmitting(false)
      alert("Thank you for your message! We will get back to you soon.")
      e.currentTarget.reset()
    }, 1000)
  }

  return (
    <>
      <MarketingNavbar />
      <main className="min-h-screen bg-background">
        <section className="relative overflow-hidden px-6 py-32 sm:px-8 sm:py-40 lg:px-12 lg:py-48">
          <div className="mx-auto max-w-6xl text-center">
            <h1 className="mb-8 text-balance text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl">
              Get in Touch
            </h1>
            <p className="mx-auto max-w-3xl text-lg leading-relaxed text-foreground/70 sm:text-xl">
              Have questions? We&apos;d love to help. Send us a message and we&apos;ll get back to you as soon as possible.
            </p>
          </div>
        </section>

        <section className="bg-foreground/2 px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="grid grid-cols-1 gap-12 lg:grid-cols-3 lg:gap-16">
              <div className="space-y-8">
                {[
                  { icon: Mail, title: "Email", content: "hello@liftops.com" },
                  { icon: Phone, title: "Phone", content: "+1 (555) 123-4567" },
                  { icon: MapPin, title: "Office", content: "San Francisco, CA\nUnited States" },
                ].map((item, index) => {
                  const Icon = item.icon
                  return (
                    <div key={index} className="flex gap-4">
                      <div className="flex-shrink-0">
                        <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10">
                          <Icon className="h-6 w-6 text-primary" />
                        </div>
                      </div>
                      <div>
                        <h3 className="mb-1 font-semibold text-foreground">{item.title}</h3>
                        <p className="whitespace-pre-line text-sm text-foreground/70">{item.content}</p>
                      </div>
                    </div>
                  )
                })}
              </div>

              <div className="lg:col-span-2">
                <form onSubmit={handleSubmit} className="space-y-6 rounded-2xl border border-border/50 bg-card p-10">
                  <div className="grid grid-cols-1 gap-6 sm:grid-cols-2">
                    <div>
                      <label htmlFor="name" className="mb-2 block text-sm font-medium text-foreground">
                        Full Name
                      </label>
                      <Input id="name" name="name" type="text" required className="rounded-lg" placeholder="John Doe" />
                    </div>
                    <div>
                      <label htmlFor="email" className="mb-2 block text-sm font-medium text-foreground">
                        Email
                      </label>
                      <Input
                        id="email"
                        name="email"
                        type="email"
                        required
                        className="rounded-lg"
                        placeholder="john@example.com"
                      />
                    </div>
                  </div>
                  <div>
                    <label htmlFor="company" className="mb-2 block text-sm font-medium text-foreground">
                      Company
                    </label>
                    <Input id="company" name="company" type="text" className="rounded-lg" placeholder="Your Company" />
                  </div>
                  <div>
                    <label htmlFor="subject" className="mb-2 block text-sm font-medium text-foreground">
                      Subject
                    </label>
                    <Input
                      id="subject"
                      name="subject"
                      type="text"
                      required
                      className="rounded-lg"
                      placeholder="How can we help?"
                    />
                  </div>
                  <div>
                    <label htmlFor="message" className="mb-2 block text-sm font-medium text-foreground">
                      Message
                    </label>
                    <Textarea
                      id="message"
                      name="message"
                      required
                      rows={5}
                      className="rounded-lg"
                      placeholder="Tell us more about your inquiry..."
                    />
                  </div>
                  <Button
                    type="submit"
                    disabled={isSubmitting}
                    className="w-full rounded-full bg-accent py-6 font-semibold text-accent-foreground hover:bg-accent/90"
                  >
                    {isSubmitting ? "Sending..." : "Send Message"}
                  </Button>
                </form>
              </div>
            </div>
          </div>
        </section>

        <section className="px-6 py-24 sm:px-8 sm:py-32 lg:px-12">
          <div className="mx-auto max-w-4xl">
            <div className="rounded-2xl border border-border/50 bg-card p-12 text-center sm:p-16">
              <h2 className="mb-4 text-3xl font-bold text-foreground sm:text-4xl">We typically respond within 24 hours</h2>
              <p className="text-lg text-foreground/70">
                For urgent inquiries, please call us directly at{" "}
                <span className="font-semibold">+1 (555) 123-4567</span>
              </p>
            </div>
          </div>
        </section>

        <footer className="border-t border-border/30 bg-background px-6 py-12 sm:px-8 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-8 flex items-center gap-2">
              <div className="flex h-8 w-8 items-center justify-center rounded-lg bg-primary">
                <span className="text-sm font-bold text-primary-foreground">L</span>
              </div>
              <span className="text-lg font-semibold text-foreground">LiftOps</span>
            </div>
            <p className="max-w-2xl text-sm text-foreground/50">
              Platform for elevator companies to streamline installations, maintenance, emergency response, and inventory
              management.
            </p>
            <div className="mt-8 flex flex-col gap-6 border-t border-border/30 pt-8 text-sm text-foreground/60 sm:flex-row">
              <p>&copy; 2024 LiftOps. All rights reserved.</p>
              <div className="flex gap-6">
                <Link href="#" className="transition-colors hover:text-foreground">
                  Privacy
                </Link>
                <Link href="#" className="transition-colors hover:text-foreground">
                  Terms
                </Link>
                <Link href="/contact" className="transition-colors hover:text-foreground">
                  Contact
                </Link>
              </div>
            </div>
          </div>
        </footer>
      </main>
    </>
  )
}
