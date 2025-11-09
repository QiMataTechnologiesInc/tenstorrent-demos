output "demo_bucket_name" {
  description = "Name of the demo S3 bucket"
  value       = module.demo.bucket_name
}

output "demo_bucket_arn" {
  description = "ARN of the demo S3 bucket"
  value       = module.demo.bucket_arn
}
