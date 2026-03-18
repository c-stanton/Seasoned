import { describe, it, expect, vi, beforeEach } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import LoginPage from "@/pages/login.vue" 

const vuetify = createVuetify({ components, directives })

// Standard Mocks
global.ResizeObserver = class ResizeObserver {
  observe() {} unobserve() {} disconnect() {}
};

vi.stubGlobal('useRuntimeConfig', () => ({
  public: { apiBase: 'http://localhost:5000/' }
}))

const mockFetch = vi.fn()
vi.stubGlobal('$fetch', mockFetch)

const mockNavigate = vi.fn()
vi.stubGlobal('navigateTo', mockNavigate)

// Mock Nuxt's useState
vi.stubGlobal('useState', () => ({ value: false }))

describe('LoginPage.vue', () => {
  beforeEach(() => {
    vi.clearAllMocks()
  })

  const mountOptions = {
    global: { plugins: [vuetify] }
  }

  it('switches between Login and Register modes', async () => {
    const wrapper = mount(LoginPage, mountOptions)
    
    // Default is Login
    expect(wrapper.find('h1').text()).toBe('Sign In')
    expect(wrapper.find('input[label="Confirm Password"]').exists()).toBe(false)

    // Click toggle
    await wrapper.find('.auth-toggle-btn').trigger('click')
    
    expect(wrapper.find('h1').text()).toBe('Join Us')
    // V-expand-transition might need a tick or we check the v-if logic
    expect(wrapper.vm.isLogin).toBe(false)
  })

  it('shows error if passwords do not match in registration mode', async () => {
    const wrapper = mount(LoginPage, mountOptions)
    const vm = wrapper.vm as any
    
    vm.isLogin = false
    vm.email = 'test@test.com'
    vm.password = 'password123'
    vm.confirmPassword = 'differentPassword'
    
    await vm.handleAuth()

    expect(vm.errorMessage).toBe('Passwords do not match.')
    expect(mockFetch).not.toHaveBeenCalled()
  })

  it('calls login API and redirects on success', async () => {
    mockFetch.mockResolvedValueOnce({ token: 'fake-token' })
    const wrapper = mount(LoginPage, mountOptions)
    const vm = wrapper.vm as any
    
    vm.email = 'chef@seasoned.com'
    vm.password = 'secret'
    
    await vm.handleAuth()

    expect(mockFetch).toHaveBeenCalledWith(
      expect.stringContaining('api/auth/login'),
      expect.any(Object)
    )
    expect(mockNavigate).toHaveBeenCalledWith('/')
  })

  it('displays specific error for 401 Unauthorized', async () => {
    mockFetch.mockRejectedValueOnce({ status: 401 })
    const wrapper = mount(LoginPage, mountOptions)
    const vm = wrapper.vm as any
    
    await vm.handleAuth()

    expect(vm.errorMessage).toContain('Invalid email or password')
  })
})