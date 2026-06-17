"use client"

import type { FormEvent } from "react"
import { useState } from "react"
import Link from "next/link"
import { MarketingNavbar } from "@/components/marketing/navbar"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Textarea } from "@/components/ui/textarea"
import { Mail, Phone, MapPin } from "lucide-react"
import { Logo } from "@/components/marketing/logo"

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
      <main className="min-h-screen bg-background relative overflow-hidden">
        {/* Futuristic Industrial Background Glow */}
        <div className="absolute top-0 left-1/2 -translate-x-1/2 -z-10 h-[500px] w-full max-w-7xl overflow-hidden opacity-30 pointer-events-none">
          <div className="absolute top-[-100px] left-[20%] h-[400px] w-[400px] rounded-full bg-accent/15 blur-[100px]" />
        </div>

        <section className="relative px-6 py-28 sm:px-8 sm:py-36 lg:px-12 lg:py-44">
          <div className="mx-auto max-w-6xl text-center">
            <h1 className="mb-8 text-balance text-5xl font-bold leading-tight text-foreground sm:text-6xl lg:text-7xl tracking-tight">
              Get in Touch
            </h1>
            <p className="mx-auto max-w-3xl text-lg leading-relaxed text-foreground/75 sm:text-xl">
              Have questions? We&apos;d love to help. Send us a message and we&apos;ll get back to you as soon as possible.
            </p>
          </div>
        </section>

        <section className="bg-secondary/40 px-6 py-24 sm:px-8 sm:py-32 lg:px-12 border-y border-border/50">
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
                    <div key={index} className="flex gap-4 group">
                      <div className="flex-shrink-0">
                        <div className="flex h-12 w-12 items-center justify-center rounded-lg bg-primary/10 border border-primary/10 transition-colors duration-300 group-hover:bg-accent/15 group-hover:border-accent/30">
                          <Icon className="h-6 w-6 text-primary group-hover:text-accent transition-colors duration-200" />
                        </div>
                      </div>
                      <div>
                        <h3 className="mb-1 font-semibold text-foreground group-hover:text-accent transition-colors duration-200">{item.title}</h3>
                        <p className="whitespace-pre-line text-sm text-foreground/75 leading-relaxed">{item.content}</p>
                      </div>
                    </div>
                  )
                })}
              </div>

              <div className="lg:col-span-2">
                <form onSubmit={handleSubmit} className="space-y-6 rounded-2xl border border-border bg-card p-10 shadow-xl relative overflow-hidden">
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
                    className="w-full rounded-full bg-accent py-6 font-semibold text-accent-foreground hover:bg-accent/95 cursor-pointer shadow-lg shadow-accent/20 transition-all duration-200 hover:scale-[1.01]"
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
            <div className="rounded-2xl border border-border bg-card p-12 text-center sm:p-16 shadow-lg relative overflow-hidden transition-all duration-300 hover:border-accent/30 hover:shadow-xl">
              <div className="absolute -right-10 -bottom-10 h-32 w-32 rounded-full bg-accent/5 blur-[40px] pointer-events-none" />
              <h2 className="mb-4 text-3xl font-bold text-foreground sm:text-4xl tracking-tight">We typically respond within 24 hours</h2>
              <p className="text-lg text-foreground/75">
                For urgent inquiries, please call us directly at{" "}
                <span className="font-semibold text-accent transition-colors duration-200 hover:opacity-90">+1 (555) 123-4567</span>
              </p>
            </div>
          </div>
        </section>

        <footer className="border-t border-border/30 bg-background px-6 py-12 sm:px-8 lg:px-12">
          <div className="mx-auto max-w-6xl">
            <div className="mb-8 flex items-center gap-3">
              <Logo size={36} />
              <div className="flex flex-col">
                <span className="text-xl font-black tracking-tight leading-none">
                  <span className="text-foreground">Lift</span>
                  <span className="text-accent">Ops</span>
                </span>
                <div className="h-[1px] w-full bg-accent/25 my-1" />
                <span className="text-[8px] font-bold tracking-[0.18em] text-accent uppercase leading-none">
                  Elevator Management
                </span>
              </div>
            </div>
            <p className="max-w-2xl text-sm text-foreground/60 leading-relaxed">
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
